using Journaway.Domain.Common;

namespace Journaway.Domain.ValueObjects;

/// <summary>
/// Natural identity as specified by requirements:
/// Surname + FirstName + DateOfBirth.
/// </summary>
public sealed record TravellerIdentity
{
    public string Surname { get; }
    public string FirstName { get; }
    public DateOnly DateOfBirth { get; }

    private TravellerIdentity(string surname, string firstName, DateOnly dateOfBirth)
    {
        Surname = surname;
        FirstName = firstName;
        DateOfBirth = dateOfBirth;
    }

    public static TravellerIdentity Create(string surname, string firstName, DateOnly dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(surname))
            throw new DomainException("Traveller surname must be provided.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Traveller first name must be provided.");

        var s = surname.Trim();
        var f = firstName.Trim();

        if (s.Length > 200)
            throw new DomainException("Traveller surname is too long.");

        if (f.Length > 200)
            throw new DomainException("Traveller first name is too long.");

        // Minimal DOB sanity checks (safe, not over-opinionated)
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow.Date))
            throw new DomainException("Date of birth cannot be in the future.");
        
        if (dateOfBirth < DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-120)))
            throw new DomainException("Date of birth is not valid.");

        return new TravellerIdentity(s, f, dateOfBirth);
    }

    public override string ToString() => $"{Surname}, {FirstName} ({DateOfBirth:yyyy-MM-dd})";
}