namespace Journaway.Infrastructure.Persistence.Entities;

public class RoomEntity
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    
    public string RoomCode { get; set; } = default!;
    public int BedCount { get; set; }
}