using System.Reflection;
using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Persistence.Context;

public class LinkUpContext : DbContext
{
    DbSet<Post> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<FriendShipRequest> FriendshipRequests { get; set; }
    DbSet<FriendShip> FriendShips { get; set; }
    DbSet<Like> Likes { get; set; }
    
    public DbSet<BattleshipGame> BattleshipGames { get; set; }
    public DbSet<Ship> Ships { get; set; }
    public DbSet<ShipPosition> ShipPositions { get; set; }
    public DbSet<Attack> Attacks { get; set; }
    
    public LinkUpContext(DbContextOptions<LinkUpContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}