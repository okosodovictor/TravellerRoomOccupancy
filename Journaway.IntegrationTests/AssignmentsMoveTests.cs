using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Journaway.Infrastructure.Persistence;
using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Journaway.IntegrationTests;

public sealed class AssignmentsMoveTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public AssignmentsMoveTests(TestAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Move_should_return_409_when_target_room_is_full()
    {
        await DbMigration.MigrateAsync(_factory.Services);

        var client = _factory.CreateClient();

        var hotelId = Guid.NewGuid();
        var date = new DateOnly(2026, 01, 13);

        await SeedMoveScenarioAsync(hotelId, date);

        // Try moving John from 0102 -> 0101, but 0101 is full (bedCount=1 and already has Jane)
        var req = new MoveTravellerRequest(
            HotelId: hotelId,
            Date: date,
            Traveller: new TravellerKeyDto("A12345", "Doe", "John", new DateOnly(1990, 01, 01)),
            FromRoomCode: "0102",
            ToRoomCode: "0101"
        );

        var resp = await client.PostAsJsonAsync("/api/assignments/move", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await resp.Content.ReadFromJsonAsync<ApiError>();
        body!.Code.Should().Be("room_full");
    }

    private async Task SeedMoveScenarioAsync(Guid hotelId, DateOnly date)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OccupancyDbContext>();

        var room0101 = new RoomEntity() { Id = Guid.NewGuid(), HotelId = hotelId, RoomCode = "0101", BedCount = 1 };
        var room0102 = new RoomEntity() { Id = Guid.NewGuid(), HotelId = hotelId, RoomCode = "0102", BedCount = 2 };

        var group = new TravelGroupEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            GroupId = "A12345",
            ArrivalDate = date,
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

        db.Rooms.AddRange(room0101, room0102);
        db.TravelGroups.Add(group);
        db.Travellers.AddRange(john, jane);
        
        db.RoomAssignments.Add(new RoomAssignmentEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Date = date,
            RoomId = room0101.Id,
            TravelGroupId = group.Id,
            TravellerId = jane.Id
        });

        // John assigned to 0102, will attempt to move to 0101
        db.RoomAssignments.Add(new RoomAssignmentEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Date = date,
            RoomId = room0102.Id,
            TravelGroupId = group.Id,
            TravellerId = john.Id
        });

        await db.SaveChangesAsync();
    }
}