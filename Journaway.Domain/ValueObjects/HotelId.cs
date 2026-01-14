namespace Journaway.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for tenant isolation.
/// </summary>
public readonly record struct HotelId(Guid Value)
{
    public static HotelId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}