namespace OrderApp.Domain.Events;

public sealed record OrderConfirmed(Guid OrderId, DateTime ConfirmedAt) : IDomainEvent
{
    public DateTime OccurredAt { get; } = ConfirmedAt;
}
