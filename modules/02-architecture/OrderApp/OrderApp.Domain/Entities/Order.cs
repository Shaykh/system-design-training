using OrderApp.Domain.Common;
using OrderApp.Domain.Events;
using OrderApp.Domain.Exceptions;
using OrderApp.Domain.ValueObjects;

namespace OrderApp.Domain.Entities;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderLine> _lines = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Money Total => _lines.Aggregate(Money.Zero(), (current, line) => current + line.LineTotal);

    private Order()
    {
    }

    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer id is required.", nameof(customerId));

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Reconstruction depuis la persistance (pas de règles métier).
    /// </summary>
    public static Order Rehydrate(
        Guid id,
        Guid customerId,
        OrderStatus status,
        DateTime createdAt,
        IEnumerable<OrderLine> lines)
    {
        var order = new Order
        {
            Id = id,
            CustomerId = customerId,
            Status = status,
            CreatedAt = createdAt
        };

        order._lines.AddRange(lines);
        return order;
    }

    public void AddLine(Guid productId, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Cannot modify a non-draft order.");

        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Only draft orders can be confirmed.");

        if (_lines.Count == 0)
            throw new DomainException("Order must contain at least one line.");

        Status = OrderStatus.Confirmed;
        Raise(new OrderConfirmed(Id, DateTime.UtcNow));
    }
}
