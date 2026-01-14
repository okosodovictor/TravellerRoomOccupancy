using Journaway.Infrastructure.Persistence;
using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journaway.WebApi.DevelomentSeeder;

public class DevDataSeeder
{
    private readonly OccupancyDbContext _db;
    public DevDataSeeder(OccupancyDbContext db) => _db = db;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _db.Rooms.AnyAsync(ct))
            return;

        var hotelId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var room0101 = new RoomEntity() { Id = Guid.NewGuid(), HotelId = hotelId, RoomCode = "0101", BedCount = 2 };
        var room0102 = new RoomEntity() { Id = Guid.NewGuid(), HotelId = hotelId, RoomCode = "0102", BedCount = 1 };

        var group = new TravelGroupEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            GroupId = "A12345",
            ArrivalDate = today,
            ExpectedTravellerCount = 2
        };

        var john = new TravellerEntity()
        {
            Id = Guid.NewGuid(), HotelId = hotelId, TravelGroupId = group.Id,
            Surname = "Doe", FirstName = "John", DateOfBirth = new DateOnly(1990, 01, 01)
        };

        var jane = new TravellerEntity()
        {
            Id = Guid.NewGuid(), HotelId = hotelId, TravelGroupId = group.Id,
            Surname = "Doe", FirstName = "Jane", DateOfBirth = new DateOnly(1991, 02, 02)
        };

        _db.Rooms.AddRange(room0101, room0102);
        _db.TravelGroups.Add(group);
        _db.Travellers.AddRange(john, jane);

        _db.RoomAssignments.Add(new RoomAssignmentEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Date = today,
            RoomId = room0101.Id,
            TravelGroupId = group.Id,
            TravellerId = john.Id
        });

        await _db.SaveChangesAsync(ct);
    }
}