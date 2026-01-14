using Journaway.Domain.ValueObjects;

namespace Journaway.Application.Repositories;

public interface ITravellerRepository
{
    Task<Guid?> FindTravellerAsync(
        HotelId hotelId,
        Guid travelGroupId,
        string surname,
        string firstName,
        DateOnly dateOfBirth,
        CancellationToken ct);
}