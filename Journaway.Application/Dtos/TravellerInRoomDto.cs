namespace Journaway.Application.Dtos;

public sealed record TravellerInRoomDto(
    string GroupId,
    string Surname,
    string FirstName,
    DateOnly DateOfBirth);