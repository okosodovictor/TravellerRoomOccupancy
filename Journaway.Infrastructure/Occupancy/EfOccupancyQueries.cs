using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Occupancy;

public class EfOccupancyQueries: IOccupancyQueries
{
    private readonly OccupancyDbContext _db;

    public EfOccupancyQueries(OccupancyDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<OccupiedRoomDto>> GetOccupiedRoomsAsync(
        Guid hotelId,
        DateOnly date,
        CancellationToken ct)
    {
        return await (
            from a in _db.RoomAssignments.AsNoTracking()
            join r in _db.Rooms.AsNoTracking() on a.RoomId equals r.Id
            where a.HotelId == hotelId && a.Date == date
            group r by new { r.RoomCode, r.BedCount } into g
            select new OccupiedRoomDto(
                g.Key.RoomCode,
                g.Key.BedCount,
                g.Count()
            )
        ).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OccupiedRoomDto>> GetRoomsByGroupAsync(
        Guid hotelId,
        string groupId,
        DateOnly date,
        CancellationToken ct)
    {
        return await (
            from a in _db.RoomAssignments.AsNoTracking()
            join r in _db.Rooms.AsNoTracking() on a.RoomId equals r.Id
            join g in _db.TravelGroups.AsNoTracking() on a.TravelGroupId equals g.Id
            where a.HotelId == hotelId
                  && a.Date == date
                  && g.GroupId == groupId
            group r by new { r.RoomCode, r.BedCount } into g
            select new OccupiedRoomDto(
                g.Key.RoomCode,
                g.Key.BedCount,
                g.Count()
            )
        ).ToListAsync(ct);
    }

    public async Task<RoomOccupancyDto?> GetRoomOccupancyAsync(
        Guid hotelId,
        string roomCode,
        DateOnly date,
        CancellationToken ct)
    {
        var room = await _db.Rooms.AsNoTracking()
            .Where(r => r.HotelId == hotelId && r.RoomCode == roomCode)
            .Select(r => new { r.Id, r.RoomCode, r.BedCount })
            .SingleOrDefaultAsync(ct);

        if (room is null)
            return null;

        var travellers = await (
            from a in _db.RoomAssignments.AsNoTracking()
            join t in _db.Travellers.AsNoTracking() on a.TravellerId equals t.Id
            join g in _db.TravelGroups.AsNoTracking() on a.TravelGroupId equals g.Id
            where a.HotelId == hotelId
                  && a.Date == date
                  && a.RoomId == room.Id
            select new TravellerInRoomDto(
                g.GroupId,
                t.Surname,
                t.FirstName,
                t.DateOfBirth
            )
        ).ToListAsync(ct);

        return new RoomOccupancyDto(
            room.RoomCode,
            room.BedCount,
            travellers
        );
    }
}