
using Microsoft.EntityFrameworkCore;
using PickUpGames.Models;

namespace PickUpGames.Services;

public class DatabaseMigrationFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app => {

            using var serviceScope = app.ApplicationServices.CreateScope();
            var dBContext = serviceScope.ServiceProvider.GetService<PickupGamesDBContext>();
            
            if (dBContext is not null) {
                 dBContext.Database.SetCommandTimeout(120);
                 dBContext.Database.Migrate();          
            }

            next(app);
        };
    }

}