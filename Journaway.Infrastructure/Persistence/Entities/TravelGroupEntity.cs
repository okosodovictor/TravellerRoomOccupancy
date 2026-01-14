namespace Journaway.Infrastructure.Persistence.Entities;

public class TravelGroupEntity
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }

    public string GroupId { get; set; } = default!;
    public DateOnly ArrivalDate { get; set; }
    public int ExpectedTravellerCount { get; set; }

    public List<TravellerEntity> Travellers { get; set; } = new();
}