using OrderApp.Domain.ValueObjects;

namespace OrderApp.Application.Abstractions;

public interface IProductCatalog
{
    Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}

public sealed record ProductSnapshot(Guid Id, string Name, Money UnitPrice, int Stock);
