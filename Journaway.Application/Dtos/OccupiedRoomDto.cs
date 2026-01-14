namespace Journaway.Application.Dtos;

public sealed record OccupiedRoomDto(string RoomCode, int BedCount, int OccupiedBeds);