namespace Journaway.Infrastructure.Persistence.Entities;

public class RoomAssignmentEntity
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }
    public DateOnly Date { get; set; }

    public Guid RoomId { get; set; }
    public RoomEntity Room { get; set; } = default!;

    //Navigation property
    public Guid TravelGroupId { get; set; }
    public TravelGroupEntity TravelGroup { get; set; } = default!;

    public Guid TravellerId { get; set; }
    public TravellerEntity Traveller { get; set; } = default!;
}