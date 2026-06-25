using MediatR;

namespace OrderApp.Application.Orders.Queries;

public sealed record ListOrdersByCustomerQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderApp.Application.Orders.Dtos.OrderReadDto>>;
