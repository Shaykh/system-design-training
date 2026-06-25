namespace OrderApp.Application.Orders.Dtos;

public sealed record OrderLineReadDto(Guid ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);

public sealed record OrderReadDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    IReadOnlyList<OrderLineReadDto> Lines);
