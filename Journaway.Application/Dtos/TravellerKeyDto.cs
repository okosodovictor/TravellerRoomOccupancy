namespace Journaway.Application.Dtos;

public sealed record TravellerKeyDto(
    string GroupId,
    string Surname,
    string FirstName,
    DateOnly DateOfBirth
);