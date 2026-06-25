using MediatR;

namespace OrderApp.Application.Orders.Commands;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest;
