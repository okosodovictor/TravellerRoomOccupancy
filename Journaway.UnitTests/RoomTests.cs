using Journaway.Domain.BusinessEntities;
using Journaway.Domain.Common;
using Journaway.Domain.ValueObjects;

namespace Journaway.UnitTests;

public class RoomTests
{
    [Fact]
    public void Create_ValidRoom_Succeeds()
    {
        var hotelId = HotelId.New();
        var roomCode = RoomCode.Parse("0101");

        var room = Room.Create(hotelId, roomCode, bedCount: 2);

        Assert.Equal(hotelId, room.HotelId);
        Assert.Equal("0101", room.RoomCode.Value);
        Assert.Equal(2, room.BedCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_InvalidBedCount_Throws(int bedCount)
    {
        var hotelId = HotelId.New();
        var roomCode = RoomCode.Parse("0101");

        Assert.Throws<DomainException>(() => Room.Create(hotelId, roomCode, bedCount));
    }

    [Fact]
    public void Create_NullRoomCode_Throws()
    {
        var hotelId = HotelId.New();
        Assert.Throws<DomainException>(() => Room.Create(hotelId, null!, 2));
    }
}