using Journaway.Domain.ValueObjects;

namespace Journaway.Application.Repositories;

public interface IRoomAssignmentRepository
{
    Task<Guid?> FindAssignmentAsync(
        HotelId hotelId,
        DateOnly date,
        Guid travellerId,
        CancellationToken ct);
    
    Task<Guid?> FindAssignedRoomAsync(
        HotelId hotelId,
        DateOnly date,
        Guid travellerId,
        CancellationToken ct);

    Task<int> CountAssignmentsAsync(
        HotelId hotelId,
        DateOnly date,
        Guid roomId,
        CancellationToken ct);

    Task LockRoomAsync(
        Guid roomId,
        CancellationToken ct);
    
    Task MoveAssignmentAsync(
        Guid assignmentId,
        Guid targetRoomId,
        CancellationToken ct);
    
    Task CreateAssignmentAsync(
        HotelId hotelId,
        DateOnly date,
        Guid roomId,
        Guid travelGroupId,
        Guid travellerId,
        CancellationToken ct);
}