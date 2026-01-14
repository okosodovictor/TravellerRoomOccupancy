using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Application.Repositories;

public interface ITravelGroupRepository
{
    /// <summary>Find a travel group by its group identifier.</summary>
    Task<Guid?> FindTravelGroupAsync(
        HotelId hotelId,
        GroupId groupId,
        CancellationToken ct);
}