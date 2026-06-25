using MediatR;
using OrderApp.Application.Abstractions;
using OrderApp.Domain.Entities;
using OrderApp.Domain.Exceptions;
using OrderApp.Domain.ValueObjects;

namespace OrderApp.Application.Orders.Commands;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IProductCatalog productCatalog,
    ICustomerService customerService) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerService.GetCustomerAsync(request.CustomerId, cancellationToken)
            ?? throw new DomainException($"Customer '{request.CustomerId}' was not found.");

        var order = Order.Create(request.CustomerId);
        var runningTotal = Money.Zero();

        foreach (var line in request.Lines)
        {
            var product = await productCatalog.GetProductAsync(line.ProductId, cancellationToken)
                ?? throw new DomainException($"Product '{line.ProductId}' was not found.");

            if (product.Stock < line.Quantity)
                throw new InsufficientStockException(line.ProductId);

            order.AddLine(product.Id, line.Quantity, product.UnitPrice);
            runningTotal += product.UnitPrice * line.Quantity;
            await productCatalog.ReserveStockAsync(product.Id, line.Quantity, cancellationToken);
        }

        if (runningTotal.Amount > customer.CreditLimit)
            throw new DomainException("Credit limit exceeded.");

        await orderRepository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
