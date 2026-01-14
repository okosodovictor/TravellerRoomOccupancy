using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Journaway.Application.Repositories;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Application.UseCases;

public class MoveTravellerUseCase: IMoveTravellerUseCase
{
   private readonly IUnitOfWork _unitOfWork;
    private readonly ITravelGroupRepository _travelGroups;
    private readonly ITravellerRepository _travellers;
    private readonly IRoomRepository _rooms;
    private readonly IRoomAssignmentRepository _assignments;

    public MoveTravellerUseCase(
        IUnitOfWork unitOfWork,
        ITravelGroupRepository travelGroups,
        ITravellerRepository travellers,
        IRoomRepository rooms,
        IRoomAssignmentRepository assignments)
    {
        _unitOfWork = unitOfWork;
        _travelGroups = travelGroups;
        _travellers = travellers;
        _rooms = rooms;
        _assignments = assignments;
    }

    public async Task<ApiError?> ExecuteAsync(MoveTravellerRequest request, CancellationToken ct)
    {
        // Fast-fail validation that doesn't require DB
        if (request.HotelId == Guid.Empty)
            return new ApiError("invalid_request", "HotelId is required.");

        if (string.IsNullOrWhiteSpace(request.FromRoomCode) || string.IsNullOrWhiteSpace(request.ToRoomCode))
            return new ApiError("invalid_request", "Both FromRoomCode and ToRoomCode are required.");

        if (string.Equals(request.FromRoomCode, request.ToRoomCode, StringComparison.OrdinalIgnoreCase))
            return new ApiError("invalid_request", "FromRoomCode and ToRoomCode must be different.");

        if (request.Traveller is null)
        {
            return new ApiError("invalid_request", "Traveller must be provided.");
        }

        ApiError? error = null;

        await _unitOfWork.ExecuteSerializableAsync(async innerCt =>
        {
            var hotelId = new HotelId(request.HotelId);

            GroupId groupId;
            RoomCode fromRoomCode;
            RoomCode toRoomCode;

            try
            {
                groupId = GroupId.Parse(request.Traveller.GroupId);
                fromRoomCode = RoomCode.Parse(request.FromRoomCode);
                toRoomCode = RoomCode.Parse(request.ToRoomCode);
            }
            catch (Exception ex)
            {
                // treat any parsing error as invalid request for the API layer.
                error = new ApiError("invalid_request", ex.Message);
                return;
            }

            // Resolve travel group
            var travelGroupId = await _travelGroups.FindTravelGroupAsync(hotelId, groupId, innerCt);
            if (travelGroupId is null)
            {
                error = new ApiError("group_not_found", "Travel group not found.");
                return;
            }

            // Resolve traveller (identified by surname+firstname+dob within the group)
            var travellerId = await _travellers.FindTravellerAsync(
                hotelId,
                travelGroupId.Value,
                request.Traveller.Surname,
                request.Traveller.FirstName,
                request.Traveller.DateOfBirth,
                innerCt);

            if (travellerId is null)
            {
                error = new ApiError("traveller_not_found", "Traveller not found in the specified travel group.");
                return;
            }

            // Resolve rooms
            var fromRoomId = await _rooms.FindRoomAsync(hotelId, fromRoomCode, innerCt);
            if (fromRoomId is null)
            {
                error = new ApiError("from_room_not_found", "Source room not found.");
                return;
            }

            var toRoom = await _rooms.FindRoomWithCapacityAsync(hotelId, toRoomCode, innerCt);
            if (toRoom is null)
            {
                error = new ApiError("to_room_not_found", "Target room not found.");
                return;
            }

            // Lock target room to prevent concurrent over-occupancy decisions
            await _assignments.LockRoomAsync(toRoom.Value.RoomId, innerCt);

            // Load current assignment (for the date)
            var assignmentId = await _assignments.FindAssignmentAsync(hotelId, request.Date, travellerId.Value, innerCt);
            if (assignmentId is null)
            {
                error = new ApiError("assignment_not_found", "Traveller has no assignment for the specified date.");
                return;
            }

            var assignedRoomId = await _assignments.FindAssignedRoomAsync(hotelId, request.Date, travellerId.Value, innerCt);
            if (assignedRoomId is null)
            {
                // Defensive: should not happen if FindAssignmentAsync succeeded, but keep it robust
                error = new ApiError("assignment_not_found", "Traveller has no assignment for the specified date.");
                return;
            }

            if (assignedRoomId.Value != fromRoomId.Value)
            {
                error = new ApiError("wrong_source_room", "Traveller is not assigned to the specified source room.");
                return;
            }

            // 6) Capacity check on the target room for that date
            var occupied = await _assignments.CountAssignmentsAsync(hotelId, request.Date, toRoom.Value.RoomId, innerCt);
            if (occupied >= toRoom.Value.BedCount)
            {
                error = new ApiError("room_full", "Target room is already at full capacity.");
                return;
            }

            // 7) Execute move
            await _assignments.MoveAssignmentAsync(assignmentId.Value, toRoom.Value.RoomId, innerCt);

            // SaveChanges is handled by UnitOfWork
        }, ct);

        return error;
    }
}