using OrderApp.Domain.ValueObjects;

namespace OrderApp.Domain.Entities;

public sealed class OrderLine
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    public Money UnitPrice { get; }

    public Money LineTotal => UnitPrice * Quantity;

    public OrderLine(Guid productId, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product id is required.", nameof(productId));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
