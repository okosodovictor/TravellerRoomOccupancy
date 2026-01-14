namespace Journaway.Application.Dtos;

public sealed record AssignTravellerRequestDto(
    Guid HotelId,
    DateOnly Date,
    TravellerKeyDto Traveller,
    string RoomCode
);