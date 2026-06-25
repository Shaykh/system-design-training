namespace OrderApp.Domain.Exceptions;

public sealed class InsufficientStockException(Guid productId)
    : DomainException($"Insufficient stock for product '{productId}'.");
