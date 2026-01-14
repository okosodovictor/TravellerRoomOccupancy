namespace Journaway.Application.Dtos;

public sealed record MoveTravellerRequest(
    Guid HotelId,
    DateOnly Date,
    TravellerKeyDto Traveller,
    string FromRoomCode,
    string ToRoomCode
);