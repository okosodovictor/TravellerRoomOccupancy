using Journaway.Domain.Common;
using Journaway.Domain.ValueObjects;

namespace Journaway.UnitTests;

public class RoomCodeTests
{
    [Theory]
    [InlineData("0001")]
    [InlineData("0101")]
    [InlineData("9999")]
    public void Parse_ValidRoomCode_Succeeds(string input)
    {
        var code = RoomCode.Parse(input);
        Assert.Equal(input, code.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("12A4")]
    [InlineData("12-4")]
    public void Parse_InvalidRoomCode_Throws(string input)
    {
        Assert.Throws<DomainException>(() => RoomCode.Parse(input!));
    }
}