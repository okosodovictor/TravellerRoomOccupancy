namespace Journaway.Infrastructure.Persistence.Entities;

public class TravellerEntity
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }

    public Guid TravelGroupId { get; set; }
    public TravelGroupEntity TravelGroup { get; set; } = default!;

    public string Surname { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
}