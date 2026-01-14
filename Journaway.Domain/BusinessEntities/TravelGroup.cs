using Journaway.Domain.Common;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Domain.BusinessEntities;

public class TravelGroup
{
    private readonly List<Traveller> _travellers = new();

    public HotelId HotelId { get; }
    public GroupId GroupId { get; }
    public DateOnly ArrivalDate { get; }
    public int ExpectedTravellerCount { get; }

    public IReadOnlyList<Traveller> Travellers => _travellers;

    private TravelGroup(HotelId hotelId, GroupId groupId, DateOnly arrivalDate, int expectedTravellerCount)
    {
        if (expectedTravellerCount <= 0)
            throw new DomainException("TravelGroup expected traveller count must be greater than 0.");

        HotelId = hotelId;
        GroupId = groupId;
        ArrivalDate = arrivalDate;
        ExpectedTravellerCount = expectedTravellerCount;
    }

    public static TravelGroup Create(HotelId hotelId, GroupId groupId, DateOnly arrivalDate,
        int expectedTravellerCount)
        => new(hotelId, groupId, arrivalDate, expectedTravellerCount);

    public void AddTraveller(Traveller traveller)
    {
        if (traveller is null)
            throw new DomainException("Traveller must be provided.");

        if (traveller.HotelId != HotelId)
            throw new DomainException("Traveller hotel does not match travel group hotel.");

        if (traveller.GroupId != GroupId)
            throw new DomainException("Traveller does not belong to this travel group.");

        if (_travellers.Count >= ExpectedTravellerCount)
            throw new DomainException("TravelGroup cannot exceed its expected traveller count.");

        if (_travellers.Any(t => t.Identity == traveller.Identity))
            throw new DomainException("Traveller already exists in this travel group.");

        _travellers.Add(traveller);
    }

    public void RemoveTraveller(TravellerIdentity identity)
    {
        var index = _travellers.FindIndex(t => t.Identity == identity);
        if (index < 0)
            throw new DomainException("Traveller not found in this travel group.");

        _travellers.RemoveAt(index);
    }
}