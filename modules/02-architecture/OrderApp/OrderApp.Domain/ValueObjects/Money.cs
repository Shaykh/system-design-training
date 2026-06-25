namespace OrderApp.Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency = "EUR")
{
    public static Money Zero(string currency = "EUR") => new(0, currency);

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator *(Money money, int quantity) =>
        new(money.Amount * quantity, money.Currency);

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Cannot add amounts with different currencies.");
    }
}
