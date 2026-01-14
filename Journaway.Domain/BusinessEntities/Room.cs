using Journaway.Domain.Common;
using Journaway.Domain.ValueObjects;

namespace Journaway.Domain.BusinessEntities;

public sealed class Room
{
    public HotelId HotelId { get; }
    public RoomCode RoomCode { get; }
    public int BedCount { get; }

    private Room(HotelId hotelId, RoomCode roomCode, int bedCount)
    {
        if (bedCount <= 0)
            throw new DomainException("Room bed count must be greater than 0.");

        HotelId = hotelId;
        RoomCode = roomCode ?? throw new DomainException("Room code must be provided.");
        BedCount = bedCount;
    }

    public static Room Create(HotelId hotelId, RoomCode roomCode, int bedCount)
        => new(hotelId, roomCode, bedCount);

    public override string ToString() => $"{RoomCode.Value} (Beds: {BedCount})";
}