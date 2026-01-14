namespace Journaway.Application.Dtos;

public sealed record RoomOccupancyDto(
    string RoomCode,
    int BedCount,
    IReadOnlyList<TravellerInRoomDto> Travellers);