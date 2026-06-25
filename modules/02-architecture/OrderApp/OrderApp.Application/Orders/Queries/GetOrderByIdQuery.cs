using MediatR;

namespace OrderApp.Application.Orders.Queries;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderApp.Application.Orders.Dtos.OrderReadDto?>;
