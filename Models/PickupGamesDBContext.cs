
namespace PickUpGames.Models;

using Microsoft.EntityFrameworkCore;

public class PickupGamesDBContext : DbContext
{
    public PickupGamesDBContext(DbContextOptions<PickupGamesDBContext> options) : base(options) { }
    public DbSet<Event> Events => Set<Event>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Address> Addresses => Set<Address>();

}

