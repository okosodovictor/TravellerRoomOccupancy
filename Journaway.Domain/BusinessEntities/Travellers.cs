using Journaway.Domain.Common;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Domain.BusinessEntities;

public sealed class Traveller
{
    public HotelId HotelId { get; }
    public GroupId GroupId { get; }
    public TravellerIdentity Identity { get; }

    private Traveller(HotelId hotelId, GroupId groupId, TravellerIdentity identity)
    {
        HotelId = hotelId;
        GroupId = groupId;
        Identity = identity ?? throw new DomainException("Traveller identity must be provided.");
    }

    public static Traveller Create(HotelId hotelId, GroupId groupId, TravellerIdentity identity)
        => new(hotelId, groupId, identity);
}