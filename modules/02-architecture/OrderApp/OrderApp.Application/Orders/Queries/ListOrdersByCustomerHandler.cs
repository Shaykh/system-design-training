using MediatR;
using OrderApp.Application.Abstractions;
using OrderApp.Application.Orders.Dtos;
using OrderApp.Domain.Entities;

namespace OrderApp.Application.Orders.Queries;

public sealed class ListOrdersByCustomerHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListOrdersByCustomerQuery, IReadOnlyList<OrderReadDto>>
{
    public async Task<IReadOnlyList<OrderReadDto>> Handle(
        ListOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await orderRepository.ListByCustomerAsync(request.CustomerId, cancellationToken);
        return orders.Select(Map).ToList();
    }

    private static OrderReadDto Map(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.Total.Amount,
            order.CreatedAt,
            order.Lines.Select(line => new OrderLineReadDto(
                line.ProductId,
                line.Quantity,
                line.UnitPrice.Amount,
                line.LineTotal.Amount)).ToList());
}
