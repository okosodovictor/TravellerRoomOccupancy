using Journaway.Domain.BusinessEntities;
using Journaway.Domain.Groups;
using Journaway.Domain.ValueObjects;
using Journaway.Infrastructure.Persistence.Entities;

namespace Journaway.Infrastructure.Persistence.Mapping;

/// <summary>
/// Maps persistence records to domain entities.
/// This layer isolates EF Core models from the Domain.
/// </summary>
public static class DomainMapper
{
    public static Room ToDomain(RoomEntity record)
    {
        return Room.Create(
            new HotelId(record.HotelId),
            RoomCode.Parse(record.RoomCode),
            record.BedCount
        );
    }

    public static RoomEntity ToRecord(Room domain, Guid? id = null)
    {
        return new RoomEntity
        {
            Id = id ?? Guid.NewGuid(),
            HotelId = domain.HotelId.Value,
            RoomCode = domain.RoomCode.Value,
            BedCount = domain.BedCount
        };
    }

    // ----------------------------
    // Traveller
    // ----------------------------
    /// <summary>
    /// Maps a traveller record to a domain Traveller.
    /// GroupId must be provided explicitly to avoid relying on navigation loading.
    /// </summary>
    public static Traveller ToDomain(TravellerEntity record, GroupId groupId)
    {
        return Traveller.Create(
            new HotelId(record.HotelId),
            groupId,
            TravellerIdentity.Create(
                record.Surname,
                record.FirstName,
                record.DateOfBirth
            )
        );
    }

    // ----------------------------
    // TravelGroup (Aggregate Root)
    // ----------------------------
    public static TravelGroup ToDomain(
        TravelGroupEntity groupRecord,
        IReadOnlyCollection<TravellerEntity> travellerRecords)
    {
        var group = TravelGroup.Create(
            new HotelId(groupRecord.HotelId),
            GroupId.Parse(groupRecord.GroupId),
            groupRecord.ArrivalDate,
            groupRecord.ExpectedTravellerCount
        );

        var groupId = GroupId.Parse(groupRecord.GroupId);

        foreach (var travellerRecord in travellerRecords)
        {
            var traveller = ToDomain(travellerRecord, groupId);
            group.AddTraveller(traveller);
        }

        return group;
    }
    public static RoomAssignment ToDomain(
        RoomAssignmentEntity record,
        RoomEntity room,
        TravelGroupEntity group,
        TravellerEntity traveller)
    {
        return RoomAssignment.Create(
            new HotelId(record.HotelId),
            record.Date,
            GroupId.Parse(group.GroupId),
            RoomCode.Parse(room.RoomCode),
            TravellerIdentity.Create(traveller.Surname, traveller.FirstName, traveller.DateOfBirth)
        );
    }
    
    /// <summary>
    /// Create persistence record (requires resolved FK ids).
    /// Repositories should resolve RoomId/TravelGroupId/TravellerId first.
    /// </summary>
    public static RoomAssignmentEntity ToRecord(
        Guid id,
        Guid hotelId,
        DateOnly date,
        Guid roomId,
        Guid travelGroupId,
        Guid travellerId)
    {
        return new RoomAssignmentEntity
        {
            Id = id,
            HotelId = hotelId,
            Date = date,
            RoomId = roomId,
            TravelGroupId = travelGroupId,
            TravellerId = travellerId
        };
    }
}