using Journaway.Domain.Common;

namespace Journaway.Domain.ValueObjects;

/// <summary>
/// 4-digit numeric room code (e.g., "0412").
/// Stored as string to preserve leading zeros.
/// </summary>
public sealed record RoomCode
{
    public string Value { get; }

    private RoomCode(string value) => Value = value;

    public static RoomCode Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException("RoomCode must be provided.");

        var trimmed = input.Trim();

        if (trimmed.Length != 4)
            throw new DomainException("RoomCode must be exactly 4 digits.");

        // Must be numeric-only
        foreach (var c in trimmed)
        {
            if (!char.IsDigit(c))
                throw new DomainException("RoomCode must contain digits only.");
        }

        return new RoomCode(trimmed);
    }

    public override string ToString() => Value;
}