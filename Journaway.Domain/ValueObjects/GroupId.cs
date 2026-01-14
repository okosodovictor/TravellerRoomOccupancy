using Journaway.Domain.Common;

namespace Journaway.Domain.Groups;

/// <summary>
/// Travel Group ID: exactly 6 chars, alphanumeric, max 2 letters, must not start with '0'.
/// Normalized to uppercase.
/// </summary>
public sealed record GroupId
{
    public string Value { get; }

    private GroupId(string value) => Value = value;

    public static GroupId Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException("GroupId must be provided.");

        var trimmed = input.Trim().ToUpperInvariant();

        if (trimmed.Length != 6)
            throw new DomainException("GroupId must be exactly 6 characters.");

        if (trimmed[0] == '0')
            throw new DomainException("GroupId must not start with '0'.");

        int letterCount = 0;

        foreach (var c in trimmed)
        {
            var isDigit = char.IsDigit(c);
            var isLetter = c is >= 'A' and <= 'Z';

            if (!isDigit && !isLetter)
                throw new DomainException("GroupId must be alphanumeric only (A-Z, 0-9).");

            if (isLetter) letterCount++;
        }

        if (letterCount > 2)
            throw new DomainException("GroupId may contain a maximum of 2 letters.");

        return new GroupId(trimmed);
    }

    public override string ToString() => Value;
}