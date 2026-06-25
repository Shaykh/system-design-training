using MediatR;

namespace OrderApp.Application.Orders.Commands;

public sealed record CreateOrderLineDto(Guid ProductId, int Quantity);

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<CreateOrderLineDto> Lines) : IRequest<Guid>;
