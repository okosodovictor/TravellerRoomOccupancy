using Journaway.Domain.ValueObjects;

namespace Journaway.Application.Repositories;

public interface IRoomRepository
{
    Task<Guid?> FindRoomAsync(
        HotelId hotelId,
        RoomCode roomCode,
        CancellationToken ct);
    
    Task<(Guid RoomId, int BedCount)?> FindRoomWithCapacityAsync(
        HotelId hotelId,
        RoomCode roomCode,
        CancellationToken ct);
}