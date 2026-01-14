using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Journaway.Application.Repositories;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Application.UseCases;

public class AssignTravellerUseCase: IAssignTravellerUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITravelGroupRepository _travelGroups;
    private readonly ITravellerRepository _travellers;
    private readonly IRoomRepository _rooms;
    private readonly IRoomAssignmentRepository _assignments;

    public AssignTravellerUseCase(
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
    public async Task<ApiError> AssignTraveller(AssignTravellerRequestDto request, CancellationToken ct)
    {
        if (request.HotelId == Guid.Empty)
            return new ApiError("invalid_request", "HotelId is required.");

        if (request.Traveller is null)
            return new ApiError("invalid_request", "Traveller must be provided.");

        if (string.IsNullOrWhiteSpace(request.RoomCode))
            return new ApiError("invalid_request", "RoomCode is required.");

        ApiError? error = null;

        await _unitOfWork.ExecuteSerializableAsync(async innerCt =>
        {
            var hotelId = new HotelId(request.HotelId);

            GroupId groupId;
            RoomCode roomCode;
            try
            {
                groupId = GroupId.Parse(request.Traveller.GroupId);
                roomCode = RoomCode.Parse(request.RoomCode);
            }
            catch (Exception ex)
            {
                error = new ApiError("invalid_request", ex.Message);
                return;
            }

            // Resolve group
            var travelGroupId = await _travelGroups.FindTravelGroupAsync(hotelId, groupId, innerCt);
            if (travelGroupId is null)
            {
                error = new ApiError("group_not_found", "Travel group not found.");
                return;
            }

            // Resolve traveller
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

            // Resolve room + capacity
            var room = await _rooms.FindRoomWithCapacityAsync(hotelId, roomCode, innerCt);
            if (room is null)
            {
                error = new ApiError("room_not_found", "Room not found.");
                return;
            }

            // Lock target room to serialize capacity decisions
            await _assignments.LockRoomAsync(room.Value.RoomId, innerCt);

            // Ensure traveller is not already assigned on that date
            var existing = await _assignments.FindAssignmentAsync(hotelId, request.Date, travellerId.Value, innerCt);
            if (existing is not null)
            {
                error = new ApiError("already_assigned", "Traveller already has an assignment for this date.");
                return;
            }

            // Capacity check
            var occupied = await _assignments.CountAssignmentsAsync(hotelId, request.Date, room.Value.RoomId, innerCt);
            if (occupied >= room.Value.BedCount)
            {
                error = new ApiError("room_full", "Room is already at full capacity.");
                return;
            }

            // Create assignment
            await _assignments.CreateAssignmentAsync(
                hotelId: hotelId,
                date: request.Date,
                roomId: room.Value.RoomId,
                travelGroupId: travelGroupId.Value,
                travellerId: travellerId.Value,
                innerCt);

            // SaveChanges handled by UnitOfWork
        }, ct);
        
        return error;
    }
}