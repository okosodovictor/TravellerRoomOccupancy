using Journaway.Application.Repositories;
using Journaway.Domain.ValueObjects;
using Journaway.Infrastructure.Persistence;
using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Repositories;

public class EfRoomAssignmentRepository: IRoomAssignmentRepository
{
    private readonly OccupancyDbContext _db;
    public EfRoomAssignmentRepository(OccupancyDbContext db) => _db = db;

    public async Task<Guid?> FindAssignmentAsync(
        HotelId hotelId, 
        DateOnly date, 
        Guid travellerId, 
        CancellationToken ct)
    {
        return await _db.RoomAssignments.AsNoTracking()
            .Where(a => a.HotelId == hotelId.Value && a.Date == date && a.TravellerId == travellerId)
            .Select(a => (Guid?)a.Id)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<Guid?> FindAssignedRoomAsync(HotelId hotelId, DateOnly date, Guid travellerId, CancellationToken ct)
    {
        return await _db.RoomAssignments.AsNoTracking()
            .Where(a => a.HotelId == hotelId.Value && a.Date == date && a.TravellerId == travellerId)
            .Select(a => (Guid?)a.RoomId)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<int> CountAssignmentsAsync(HotelId hotelId, DateOnly date, Guid roomId, CancellationToken ct)
    {
        return await _db.RoomAssignments.AsNoTracking()
            .Where(a => a.HotelId == hotelId.Value && a.Date == date && a.RoomId == roomId)
            .CountAsync(ct);
    }

    public Task LockRoomAsync(Guid roomId, CancellationToken ct)
    {
        return _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM rooms WHERE \"Id\" = {roomId} FOR UPDATE",
            ct);
    }

    public async Task MoveAssignmentAsync(Guid assignmentId, Guid targetRoomId, CancellationToken ct)
    {
        var assignment = await _db.RoomAssignments
            .Where(a => a.Id == assignmentId)
            .SingleAsync(ct);
        
        assignment.RoomId = targetRoomId;
    }
    
    public Task CreateAssignmentAsync(
        HotelId hotelId,
        DateOnly date,
        Guid roomId,
        Guid travelGroupId,
        Guid travellerId,
        CancellationToken ct)
    {
        _db.RoomAssignments.Add(new RoomAssignmentEntity
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId.Value,
            Date = date,
            RoomId = roomId,
            TravelGroupId = travelGroupId,
            TravellerId = travellerId
        });

        return Task.CompletedTask;
    }
}