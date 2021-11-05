using Microsoft.EntityFrameworkCore;
using PickUpGames.Dtos;
using PickUpGames.Models;
using PickUpGames.Services;
using PickUpGames.Models.Mappers;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<PickupGamesDBContext>("Data Source=pickupGamesDB.db;");
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<JwtSecurityTokenHandlerFactory>();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pickup Games API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PickUpGames v1"));
}


app.UseHttpsRedirection();

//app.UseAuthentication();

app.UseAuthorization();

app.Use(async (httpContext, next) => {

   var jwtToken = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];

   if (jwtToken is null)  {
       httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
       await httpContext.Response.WriteAsJsonAsync(new {Message = "Missing Bearer token"});
   } 
  
   if (jwtToken is not null) {

      var tokenService = httpContext.RequestServices.GetService<ITokenService>();
      var claimsPrincipal  = tokenService?.ValidateToken(jwtToken);

      if (claimsPrincipal is not null) {
          httpContext.User = claimsPrincipal;
      } else {
          httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
          await httpContext.Response.WriteAsJsonAsync(new {Message = "Invalid bearer token provided"});
      }    
   }

   await next();
});

app.MapHealthChecks("/health");

app.MapPost("/users/authenticate", [AllowAnonymous] (UserAuth userAuth, IUserService userService, ITokenService tokenService) => 
{
   
     var (isAuthenticated, user) = userService.Authenticate(userAuth);

     if(!isAuthenticated || user == null) {
         return Results.BadRequest(new {Message = "Your email address or password is incorrect"});
     }

     var jwtToken = tokenService.GenerateToken(user);
     return Results.Json( new {AccessToken = jwtToken, tokenType = "Bearer" });
})
  .Accepts<UserAuth>("application/json")
  .Produces<JwtTokenDetails>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithName("Authenticate")
  .WithTags("Auth");

//Users
app.MapGet("/users", async (PickupGamesDBContext _dbContext) =>
{
    var users = await _dbContext.Users.ToListAsync<User>();
    return Results.Json(UserMapper.MapUsers(users));

}).Produces<List<UserDto>>(StatusCodes.Status200OK, "application/json")
  .WithTags("Users")
  .WithName("GetAllUsers");

app.MapGet("/users/{id}", async (int id, PickupGamesDBContext _dbContext) =>
{
    var user = await _dbContext.FindAsync<User>(id);
    if (user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    return Results.Json(UserMapper.MapUser(user));

}).Produces<UserDto>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Users")
  .WithName("GetUser");

app.MapPost("/users",  (UserDto userDto, IUserService userService) =>
{    
     var registeredUser = userService.RegisterUser(userDto);

     return Results.CreatedAtRoute("/users/{id}", new { id = registeredUser.UserId}, registeredUser);

}).Accepts<UserDto>("application/json")
  .Produces<UserDto>(StatusCodes.Status200OK, "application/json")
  .WithTags("Users")
  .WithName("CreateUser");

app.MapPut("/users/{id}", async (int id, User user, PickupGamesDBContext _dbContext) =>
{
    if(user == null) return Results.BadRequest();

    var userToUpdate = await _dbContext.FindAsync<User>(id);
    if(userToUpdate == null) return Results.NotFound();

    userToUpdate.FirstName = user.FirstName;
    userToUpdate.LastName = user.LastName;
    userToUpdate.Email = user.Email;
    userToUpdate.ProfileImageUrl = user.ProfileImageUrl;
    userToUpdate.LastUpdatedAt = DateTime.Now;

    await _dbContext.SaveChangesAsync();
    return Results.NoContent();

}).Accepts<UserDto>("application/json")
  .WithTags("Users")
  .WithName("UpdateUser"); ;

app.MapDelete("/users/{id}", async (int id, PickupGamesDBContext _dbContext) =>
{
    var user = await _dbContext.FindAsync<User>(id);

    if (user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found"});
    }

    _dbContext.Remove<User>(user);
    await _dbContext.SaveChangesAsync();

    return Results.NoContent();

}).Accepts<UserDto>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Users")
  .WithName("DeleteUser"); ;

//Events
app.MapGet("/events", async (PickupGamesDBContext _dbContext) =>
{
    var events = await _dbContext.Events.ToListAsync<Event>();

    return Results.Json(events);

})
.Produces<List<Event>>(StatusCodes.Status200OK, "application/json")
.WithTags("Events")
.WithName("GetAllEvents");

app.MapGet("/events/{id}", async (int id, PickupGamesDBContext _dbContext) =>
{
    var searchedEvent = await _dbContext.FindAsync<Event>(id);
    if (searchedEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found"});
    }

    return Results.Json(searchedEvent);

}).Produces<Event>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("GetEvent");

app.MapPost("/events", async (Event eventDto, PickupGamesDBContext _dbContext) =>
{
    var eventOwner = await _dbContext.FindAsync<User>(eventDto.UserId);
    if (eventOwner == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    _dbContext.Add<Event>(eventDto);
    await _dbContext.SaveChangesAsync();

    return Results.CreatedAtRoute("events/{id}", new { id = eventDto.EventId }, eventDto);

}).Accepts<Event>("application/json")
  .Produces<Event>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("CreateEvent");

app.MapPut("/events/{id}", async (int id, Event eventDto, PickupGamesDBContext _dbContext) =>
{
    if (eventDto == null) return Results.BadRequest();

    var eventToUpdate = await _dbContext.FindAsync<Event>(id);
    if (eventToUpdate == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    var eventOwner  = await _dbContext.FindAsync<User>(eventDto.UserId);
    if(eventOwner == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    eventToUpdate.Title = eventDto.Title;
    eventToUpdate.Location = eventDto.Location;
    eventToUpdate.Description = eventDto.Description;
    eventToUpdate.EventStatus = eventDto.EventStatus;
    eventToUpdate.StartDate = eventDto.StartDate;
    eventToUpdate.EndDate = eventDto.EndDate;
    eventToUpdate.StartTime = eventDto.StartTime;
    eventToUpdate.EventPrivacy = eventDto.EventPrivacy;
    eventToUpdate.EventType = eventDto.EventType;
    eventToUpdate.MaxNumberOfParticipants = eventDto.MaxNumberOfParticipants;
    eventToUpdate.UserId = eventOwner.UserId;
    eventToUpdate.LastUpdatedAt = DateTime.Now;

    await _dbContext.SaveChangesAsync();
    return Results.NoContent();

}).Accepts<Event>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("UpdateEvent");

app.MapDelete("/events/{id}", async (int id, PickupGamesDBContext _dbContext) =>
{
    var searchedEvent = await _dbContext.FindAsync<Event>(id);

    if (searchedEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    _dbContext.Remove<Event>(searchedEvent);
    await _dbContext.SaveChangesAsync();

    return Results.NoContent();

}).Accepts<Event>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Events")
  .WithName("DeleteEvent");


//Participants
app.MapPost("/participants", async (Participant participant, PickupGamesDBContext _dbContext) =>
{
    var user = await _dbContext.Users.FindAsync(participant.UserId);
    if(user == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "User not found" });
    }

    var existingEvent = await _dbContext.Events.FindAsync(participant.EventId);
    if(existingEvent == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event not found" });
    }

    _dbContext.Add<Participant>(participant);
    await _dbContext.SaveChangesAsync();

    return Results.CreatedAtRoute("participants/{id}", new { id = participant.ParticipantId }, participant);

}).Accepts<Participant>("application/json")
  .Produces<Participant>(StatusCodes.Status200OK, "application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("GetAllParticipants");

app.MapPut("/participants/{id}", async (int id, Participant participant, PickupGamesDBContext _dbContext) =>
{
    if (participant == null) return Results.BadRequest();

    var participantToUpdate = await _dbContext.FindAsync<Participant>(id);
    if (participantToUpdate == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event participant not found" });

    }

    participantToUpdate.UserId = participant.UserId;
    participantToUpdate.EventId = participant.EventId;
    participantToUpdate.Status = participant.Status;
    participantToUpdate.LastUpdatedAt = DateTime.Now;

    await _dbContext.SaveChangesAsync();
    return Results.NoContent();

}).Accepts<Participant>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("UpdateParticipant");

app.MapDelete("/participants/{id}", async (int id, PickupGamesDBContext _dbContext) =>
{
    var participant = await _dbContext.FindAsync<Participant>(id);
    if (participant == null)
    {
        return Results.NotFound(new NotFoundDetails { Message = "Event participant not found" });
    }

    _dbContext.Remove<Participant>(participant);
    await _dbContext.SaveChangesAsync();

    return Results.NoContent();

}).Accepts<Participant>("application/json")
  .Produces<NotFoundDetails>(StatusCodes.Status404NotFound, "application/json")
  .WithTags("Participants")
  .WithName("DeleteParticipant"); ;

app.MapControllers();

app.Run();