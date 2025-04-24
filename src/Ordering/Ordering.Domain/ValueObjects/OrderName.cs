namespace Ordering.Domain.ValueObjects;
public record OrderName
{
    private OrderName(string value) => Value = value;
    private const int DefaultLength = 5;
    public string Value { get; } = default!;
    public static OrderName Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

        return new OrderName(value);
    }
}
