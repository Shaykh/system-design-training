using MediatR;
using OrderApp.Application.Abstractions;
using OrderApp.Domain.Exceptions;

namespace OrderApp.Application.Orders.Commands;

public sealed class ConfirmOrderHandler(
    IOrderRepository orderRepository,
    IPublisher publisher) : IRequestHandler<ConfirmOrderCommand>
{
    public async Task Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new DomainException($"Order '{request.OrderId}' was not found.");

        order.Confirm();
        await orderRepository.UpdateAsync(order, cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        order.ClearDomainEvents();
    }
}
