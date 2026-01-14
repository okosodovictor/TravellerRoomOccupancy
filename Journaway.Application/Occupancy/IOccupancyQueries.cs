using Journaway.Application.Dtos;

namespace Journaway.Application.Occupancy;

public interface IOccupancyQueries
{
    Task<IReadOnlyList<OccupiedRoomDto>> GetOccupiedRoomsAsync(
        Guid hotelId,
        DateOnly date,
        CancellationToken ct);

    Task<IReadOnlyList<OccupiedRoomDto>> GetRoomsByGroupAsync(
        Guid hotelId,
        string groupId,
        DateOnly date,
        CancellationToken ct);

    Task<RoomOccupancyDto?> GetRoomOccupancyAsync(
        Guid hotelId,
        string roomCode,
        DateOnly date,
        CancellationToken ct);
}