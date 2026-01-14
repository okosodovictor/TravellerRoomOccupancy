using Journaway.Domain.Common;
using Journaway.Domain.ValueObjects;

namespace Journaway.UnitTests;

public class TravellerIdentityTests
{
    [Fact]
    public void Create_ValidIdentity_Succeeds()
    {
        var dob = new DateOnly(1995, 4, 12);

        var id = TravellerIdentity.Create("Doe", "John", dob);

        Assert.Equal("Doe", id.Surname);
        Assert.Equal("John", id.FirstName);
        Assert.Equal(dob, id.DateOfBirth);
    }

    [Theory]
    [InlineData(null, "John")]
    [InlineData("", "John")]
    [InlineData("   ", "John")]
    [InlineData("Doe", null)]
    [InlineData("Doe", "")]
    [InlineData("Doe", "   ")]
    public void Create_MissingNames_Throws(string surname, string firstName)
    {
        var dob = new DateOnly(1995, 4, 12);
        Assert.Throws<DomainException>(() => TravellerIdentity.Create(surname!, firstName!, dob));
    }

    [Fact]
    public void Create_FutureDob_Throws()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));
        Assert.Throws<DomainException>(() => TravellerIdentity.Create("Doe", "John", tomorrow));
    }

    [Fact]
    public void Create_TrimsNames()
    {
        var dob = new DateOnly(1995, 4, 12);
        var id = TravellerIdentity.Create("  Doe  ", "  John  ", dob);

        Assert.Equal("Doe", id.Surname);
        Assert.Equal("John", id.FirstName);
    }
}