using Microsoft.EntityFrameworkCore;
using PickUpGames.Dtos;
using PickUpGames.Models;
using PickUpGames.Services;
using PickUpGames.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using PickUpGames.Auth;
using PickUpGames.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;
using Models.Mappers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<PickupGamesDBContext>("Data Source=pickupGamesDB.db;");
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<JwtSecurityTokenHandlerFactory>();
builder.Services.AddResponseCaching();
builder.Services.AddControllers();

if (builder.Environment.IsProduction()) {

   builder.Services.AddTransient<IStartupFilter, DatabaseMigrationFilter>();
}

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddCors();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; 
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.Configure<JsonOptions>(options => {
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; 
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

//My custom jwt - Note. without passing the scheme name, authz fails
builder.Services.AddAuthentication("CustomJwt").AddScheme<CustomJwtAuthenticationOptions, CustomJwtAuthenticationHandler>("CustomJwt", null);

builder.Services.AddAuthorization(options =>
{
    //Good way for creating auth policies (collection of requirements to be met) for a grouped of endpoints
    //However, not great for checking claims for single endpoints. Users may need this for Minimal
    // Add defaults claims name for jwt
    // Can we reconcile the ClaimsConstants ?
    //options.AddPolicy("ApiPolicy", policy => policy.RequireClaim(ClaimConstants.Scope, "api:read", "api:create", "api:update", "api:delete"));
   // options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pickup Games API", Version = "v1" });
});

//builder.Services.AddConvey();

//builder.Services.AddUserServices(); 

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PickUpGames v1"));

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(opts => opts.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

//app.UseEtagResponseCaching();

app.MapHealthChecks("/health");

app.MapHub<GameEventHub>("/eventMessage");


app.MapPost("/users/authenticate", [AllowAnonymous] (UserAuth userAuth, IUserService userService, ITokenService tokenService) =>
{

    var (isAuthenticated, user) = userService.Authenticate(userAuth);

    if (!isAuthenticated || user == null)
    {
        return Results.BadRequest(new { Message = "Your email address or password is incorrect" });
    }

    var jwtToken = tokenService.GenerateToken(user);
    return Results.Json(new { AccessToken = jwtToken, tokenType = "Bearer" });
})
  .Accepts<UserAuth>("application/json")
  .Produces<JwtTokenDetails>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithName("Authenticate")
  .WithTags("Auth");

//Users
app.MapGet("/users", async (HttpContext context, PickupGamesDBContext dbContext) =>
{
    //var ClaimsPrinciple = context.User;
    //var users = await dbContext.Users.ToListAsync<User>();
    var users = await dbContext.Users.Include(u => u.Events).ToListAsync();
    return Results.Json(UserMapper.MapUsers(users));

})
  .Produces<List<UserDTO>>(StatusCodes.Status200OK, "application/json")
  .WithTags("Users")
  .WithName("GetAllUsers");

app.MapGet("/users/{id}", async (int id, PickupGamesDBContext dbContext) =>
{
    var user = await dbContext.Users.Include(u => u.Events).FirstOrDefaultAsync(u => u.UserId == id);
    if (user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    return Results.Json(UserMapper.MapUser(user));

}).RequireAuthorization()
  .Produces<UserDTO>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Users")
  .WithName("GetUser");

app.MapPost("/users", [AllowAnonymous] (UserRequestDTO userDto, IUserService userService) =>
{
   var registeredUser = userService.RegisterUser(userDto);

   return Results.Created($"/users/{registeredUser.UserId}", registeredUser);

})
  .Accepts<UserRequestDTO>("application/json")
  .Produces<UserDTO>(StatusCodes.Status200OK, "application/json")
  .WithTags("Users")
  .WithName("CreateUser");

app.MapPut("/users/{id}", async (int id, UserRequestDTO user, PickupGamesDBContext dbContext) =>
{
    if (user == null) return Results.BadRequest();

    var userToUpdate = await dbContext.FindAsync<User>(id);
    if (userToUpdate == null) return Results.NotFound();

    userToUpdate.FirstName = user.FirstName;
    userToUpdate.LastName = user.LastName;
    userToUpdate.Email = user.Email;
    userToUpdate.ProfileImageUrl = user.ProfileImageUrl;
    userToUpdate.LastUpdatedAt = DateTime.Now;

    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization()
  .Accepts<UserRequestDTO>("application/json")
  .WithTags("Users")
  .WithName("UpdateUser"); ;

app.MapDelete("/users/{id}", async (int id, PickupGamesDBContext dbContext) =>
{
    var user = await dbContext.FindAsync<User>(id);

    if (user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    dbContext.Remove<User>(user);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();

}).RequireAuthorization()
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Users")
  .WithName("DeleteUser"); ;

//Events
app.MapGet("/events",  [AllowAnonymous] async (PickupGamesDBContext dbContext) =>
{
    var allEvents = await dbContext
                          .Events
                          .Include(ev => ev.User)
                          .Include(e => e.Address)
                          .ToListAsync();
    var mappedEvents = EventMapper.MapEvents(allEvents);

    return Results.Json(mappedEvents);
  
})
.Produces<List<EventDTO>>(StatusCodes.Status200OK, "application/json")
.WithTags("Events")
.WithName("GetAllEvents");

app.MapGet("/events/{id}", async (int id, PickupGamesDBContext dbContext) =>
{
    var searchedEvent = await dbContext.Events
                                       .Include(ev => ev.User)
                                       .Include(ev => ev.Participants)
                                       .FirstOrDefaultAsync(ev => ev.EventId == id);
    if (searchedEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    return Results.Ok(EventMapper.MapEventWithOwner(searchedEvent));

})
  .RequireAuthorization()
  .Produces<EventDTO>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("GetEvent");

app.MapPost("/events", async (EventRequestDTO eventDto, PickupGamesDBContext dbContext, IHubContext<GameEventHub> hubContext) =>
{
    User? eventOwner = await dbContext.FindAsync<User>(eventDto.OwnerId);
    if (eventOwner == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    var sportEvent = EventMapper.MapEventRequestDTO(eventDto);
    sportEvent.User = eventOwner;
    dbContext.Add<Event>(sportEvent);
    await dbContext.SaveChangesAsync();

    //var allEvents = await dbContext.Events.ToListAsync<Event>();
    //Notify clients of new events
    //var mappedEvents = EventMapper.MapEvents(allEvents);
    //await hubContext.Clients.All.SendAsync("ReceiveEventMessages", mappedEvents);

    return Results.Ok(EventMapper.MapEventWithOwner(sportEvent));

}).RequireAuthorization()
  .Accepts<EventRequestDTO>("application/json")
  .Produces<EventDTO>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("CreateEvent");

app.MapPut("/events/{id}", async (int id, EventRequestDTO eventDto, PickupGamesDBContext dbContext) =>
{
    if (eventDto == null) return Results.BadRequest();

    var eventToUpdate = await dbContext.FindAsync<Event>(id);
    if (eventToUpdate == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    var eventOwner = await dbContext.FindAsync<User>(eventDto.OwnerId);
    if (eventOwner == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    eventToUpdate.Title = eventDto.Title;
    eventToUpdate.Address = EventMapper.MapAddressRequestDTO(eventDto.Location);
    eventToUpdate.Description = eventDto.Description;
    eventToUpdate.EventStatus = (EventStatus) eventDto.EventStatus;
    eventToUpdate.StartDate = eventDto.StartDate;
    eventToUpdate.StartTime = eventDto.StartTime;
    eventToUpdate.EventPrivacy = (EventPrivacy) eventDto.EventPrivacy;
    eventToUpdate.EventType = (EventType) eventDto.EventType;
    eventToUpdate.MaxNumberOfParticipants = eventDto.MaxNumberOfParticipants;
    eventToUpdate.UserId = eventOwner.UserId;
    eventToUpdate.LastUpdatedAt = DateTime.Now;

    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization()
  .Accepts<EventDTO>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("UpdateEvent");

app.MapDelete("/events/{id}", async (int id, PickupGamesDBContext dbContext) =>
{
    var searchedEvent = await dbContext.FindAsync<Event>(id);

    if (searchedEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    dbContext.Remove<Event>(searchedEvent);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();

}).RequireAuthorization()
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("DeleteEvent");


//Participants
app.MapPost("/participants", async (ParticipantRequestDTO participantRequestDTO, PickupGamesDBContext dbContext) =>
{
    User? user = await dbContext.Users.FindAsync(participantRequestDTO.UserId);
    if (user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    Event? existingEvent = await dbContext.Events.FindAsync(participantRequestDTO.EventId);
    if (existingEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    var participant = new Participant () {
        User = user,
        Event = existingEvent,
        EventId = existingEvent.EventId,
    };

    dbContext.Add<Participant>(participant);
    await dbContext.SaveChangesAsync();

    return Results.Created($"participants/{participant.ParticipantId}", ParticipantMapper.MapParticipant(participant));

}).RequireAuthorization()
  .Accepts<ParticipantRequestDTO>("application/json")
  .Produces<ParticipantDTO>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("GetAllParticipants");

app.MapPut("/participants/{id}", async (int id, ParticipantRequestDTO participant, PickupGamesDBContext dbContext) =>
{
    if (participant == null) return Results.BadRequest();

    var participantToUpdate = await dbContext.FindAsync<Participant>(id);
    if (participantToUpdate == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event participant not found" });

    }

    participantToUpdate.UserId = participant.UserId;
    participantToUpdate.EventId = participant.EventId;
    participantToUpdate.Status = participant.Status;
    participantToUpdate.LastUpdatedAt = DateTime.Now;

    await dbContext.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization()
  .Accepts<ParticipantRequestDTO>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("UpdateParticipant");

app.MapDelete("/participants/{id}", async (int id, PickupGamesDBContext dbContext) =>
{
    var participant = await dbContext.FindAsync<Participant>(id);
    if (participant == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event participant not found" });
    }

    dbContext.Remove<Participant>(participant);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();

}).RequireAuthorization()
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("DeleteParticipant"); ;


app.Run();