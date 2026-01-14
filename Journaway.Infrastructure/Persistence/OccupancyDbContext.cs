using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Persistence;

public class OccupancyDbContext: DbContext
{
    public OccupancyDbContext(DbContextOptions<OccupancyDbContext> options) : base(options) { }

    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<TravelGroupEntity> TravelGroups => Set<TravelGroupEntity>();
    public DbSet<TravellerEntity> Travellers => Set<TravellerEntity>();
    
    public DbSet<RoomAssignmentEntity> RoomAssignments => Set<RoomAssignmentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OccupancyDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}