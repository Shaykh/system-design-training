using MediatR;
using OrderApp.Application.Abstractions;
using OrderApp.Application.Orders.Dtos;
using OrderApp.Domain.Entities;

namespace OrderApp.Application.Orders.Queries;

public sealed class GetOrderByIdHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderReadDto?>
{
    public async Task<OrderReadDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        return order is null ? null : Map(order);
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
