using Journaway.Domain.Common;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;

namespace Journaway.Domain.BusinessEntities;

/// <summary>
/// Domain representation of assigning a traveller to a room on a specific date.
/// This is the business fact we persist and query.
/// </summary>
public sealed class RoomAssignment
{
    public HotelId HotelId { get; }
    public DateOnly Date { get; }

    public GroupId GroupId { get; }
    public RoomCode RoomCode { get; }
    public TravellerIdentity Traveller { get; }

    private RoomAssignment(
        HotelId hotelId,
        DateOnly date,
        GroupId groupId,
        RoomCode roomCode,
        TravellerIdentity traveller)
    {
        HotelId = hotelId;
        Date = date;
        GroupId = groupId;
        RoomCode = roomCode ?? throw new DomainException("Room code must be provided.");
        Traveller = traveller ?? throw new DomainException("Traveller identity must be provided.");
    }

    public static RoomAssignment Create(
        HotelId hotelId,
        DateOnly date,
        GroupId groupId,
        RoomCode roomCode,
        TravellerIdentity traveller)
    {
        if (date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            throw new DomainException("Assignment date cannot be in the past.");
        
        return new RoomAssignment(hotelId, date, groupId, roomCode, traveller);
    }
}