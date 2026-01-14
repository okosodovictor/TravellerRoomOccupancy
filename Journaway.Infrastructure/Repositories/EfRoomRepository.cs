using Journaway.Application.Repositories;
using Journaway.Domain.ValueObjects;
using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Repositories;

public class EfRoomRepository: IRoomRepository
{
    private readonly OccupancyDbContext _db;
    public EfRoomRepository(OccupancyDbContext db) => _db = db;

    public async Task<Guid?> FindRoomAsync(HotelId hotelId, RoomCode roomCode, CancellationToken ct)
    {
        return await _db.Rooms.AsNoTracking()
            .Where(r => r.HotelId == hotelId.Value && r.RoomCode == roomCode.Value)
            .Select(r => (Guid?)r.Id)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<(Guid RoomId, int BedCount)?> FindRoomWithCapacityAsync(
        HotelId hotelId, 
        RoomCode roomCode, 
        CancellationToken ct)
    {
        var row = await _db.Rooms.AsNoTracking()
            .Where(r => r.HotelId == hotelId.Value && r.RoomCode == roomCode.Value)
            .Select(r => new { r.Id, r.BedCount })
            .SingleOrDefaultAsync(ct);

        return row is null ? null : (row.Id, row.BedCount);
    }
}