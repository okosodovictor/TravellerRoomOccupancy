using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Journaway.Infrastructure.Persistence;
using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Journaway.IntegrationTests;

public sealed class AssignmentsAssignTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public AssignmentsAssignTests(TestAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Assign_should_return_409_when_room_is_full()
    {
        // Arrange
        await DbMigration.MigrateAsync(_factory.Services);

        var client = _factory.CreateClient();

        var hotelId = Guid.NewGuid();
        var date = new DateOnly(2026, 01, 13);

        // Seed: room bedCount=1, group + two travellers, and assign first traveller already
        await SeedAsync(hotelId, date, bedCount: 1, preAssignFirstTraveller: true);

        // Try assigning second traveller to same 1-bed room => should conflict room_full
        var req = new AssignTravellerRequestDto(
            HotelId: hotelId,
            Date: date,
            Traveller: new TravellerKeyDto("A12345", "Doe", "Jane", new DateOnly(1991, 02, 02)),
            RoomCode: "0101"
        );

        // Act
        var resp = await client.PostAsJsonAsync("/api/assignments/assign", req);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await resp.Content.ReadFromJsonAsync<ApiError>();
        body!.Code.Should().Be("room_full");
    }

    private async Task SeedAsync(Guid hotelId, DateOnly date, int bedCount, bool preAssignFirstTraveller)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OccupancyDbContext>();

        // Room
        var room = new RoomEntity() { Id = Guid.NewGuid(), HotelId = hotelId, RoomCode = "0101", BedCount = bedCount };

        // Group
        var group = new TravelGroupEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            GroupId = "A12345",
            ArrivalDate = date,
            ExpectedTravellerCount = 2
        };

        // Travellers
        var t1 = new TravellerEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            TravelGroupId = group.Id,
            Surname = "Doe",
            FirstName = "John",
            DateOfBirth = new DateOnly(1990, 01, 01)
        };

        var t2 = new TravellerEntity()
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            TravelGroupId = group.Id,
            Surname = "Doe",
            FirstName = "Jane",
            DateOfBirth = new DateOnly(1991, 02, 02)
        };

        db.Rooms.Add(room);
        db.TravelGroups.Add(group);
        db.Travellers.AddRange(t1, t2);

        if (preAssignFirstTraveller)
        {
            db.RoomAssignments.Add(new RoomAssignmentEntity()
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                Date = date,
                RoomId = room.Id,
                TravelGroupId = group.Id,
                TravellerId = t1.Id
            });
        }

        await db.SaveChangesAsync();
    }
}