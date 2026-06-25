using Microsoft.EntityFrameworkCore;
using OrderApp.Application.Abstractions;
using OrderApp.Domain.Exceptions;
using OrderApp.Domain.ValueObjects;
using OrderApp.Infrastructure.Persistence;

namespace OrderApp.Infrastructure.Services;

public sealed class EfProductCatalog(OrderAppDbContext dbContext) : IProductCatalog
{
    public async Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FindAsync([productId], cancellationToken);
        return product is null
            ? null
            : new ProductSnapshot(product.Id, product.Name, new Money(product.Price), product.Stock);
    }

    public async Task ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FindAsync([productId], cancellationToken)
            ?? throw new DomainException($"Product '{productId}' was not found.");

        if (product.Stock < quantity)
            throw new InsufficientStockException(productId);

        product.Stock -= quantity;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
