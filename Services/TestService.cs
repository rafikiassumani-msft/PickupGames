namespace PickUpGames.Services;


public static class UserServices {

   public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration? configuration) {
      services.AddScoped<IUserService, UserService>(); 
      return services;
   }

}
