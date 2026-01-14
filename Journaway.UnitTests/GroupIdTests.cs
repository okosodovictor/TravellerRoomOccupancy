using Journaway.Domain.Common;
using Journaway.Domain.Groups;

namespace Journaway.UnitTests;

public class GroupIdTests
{
    [Theory]
    [InlineData("A12345", "A12345")]
    [InlineData("ab1234", "AB1234")] // normalized to uppercase
    [InlineData("12AB34", "12AB34")] // 2 letters max
    [InlineData("123456", "123456")] // 0 letters
    public void Parse_ValidGroupId_Succeeds(string input, string expected)
    {
        var id = GroupId.Parse(input);
        Assert.Equal(expected, id.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]    // length 5
    [InlineData("1234567")]  // length 7
    [InlineData("0A1234")]   // starts with 0
    [InlineData("ABC123")]   // 3 letters
    [InlineData("12A!34")]   // not alphanumeric
    public void Parse_InvalidGroupId_Throws(string input)
    {
        Assert.Throws<DomainException>(() => GroupId.Parse(input!));
    }
}