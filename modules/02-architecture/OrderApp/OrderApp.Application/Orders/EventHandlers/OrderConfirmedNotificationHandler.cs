using MediatR;
using OrderApp.Application.Abstractions;
using OrderApp.Domain.Events;

namespace OrderApp.Application.Orders.EventHandlers;

public sealed class OrderConfirmedNotificationHandler(
    IOrderRepository orderRepository,
    ICustomerService customerService,
    INotificationService notificationService) : INotificationHandler<OrderConfirmed>
{
    public async Task Handle(OrderConfirmed notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(notification.OrderId, cancellationToken);
        if (order is null)
            return;

        var customer = await customerService.GetCustomerAsync(order.CustomerId, cancellationToken);
        if (customer is null)
            return;

        await notificationService.SendOrderConfirmationAsync(
            order.Id,
            customer.Email,
            order.Total.Amount,
            cancellationToken);
    }
}
