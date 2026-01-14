using Journaway.Application.Repositories;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;
using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Repositories;

public class EfTravelGroupRepository: ITravelGroupRepository
{
    private readonly OccupancyDbContext _db;
    
    public EfTravelGroupRepository(OccupancyDbContext db) => _db = db;

    public async Task<Guid?> FindTravelGroupAsync(HotelId hotelId, GroupId groupId, CancellationToken ct)
    {
        var gid = groupId.Value.Trim().ToUpperInvariant();

        return await _db.TravelGroups.AsNoTracking()
            .Where(g => g.HotelId == hotelId.Value && g.GroupId == gid)
            .Select(g => (Guid?)g.Id)
            .SingleOrDefaultAsync(ct);
    }
}