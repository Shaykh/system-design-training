using MediatR;

namespace OrderApp.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}
