using Microsoft.EntityFrameworkCore;
using OrderApp.Application.Abstractions;
using OrderApp.Domain.Entities;
using OrderApp.Domain.ValueObjects;
using OrderApp.Infrastructure.Persistence.Entities;

namespace OrderApp.Infrastructure.Persistence;

public sealed class EfOrderRepository(OrderAppDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Orders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<IReadOnlyList<Order>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.Orders
            .Include(x => x.Lines)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Add(MapToEntity(order));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Orders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"Order '{order.Id}' was not found.");

        entity.Status = order.Status.ToString();
        dbContext.OrderLines.RemoveRange(entity.Lines);
        entity.Lines = order.Lines.Select(line => new OrderLineEntity
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ProductId = line.ProductId,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice.Amount
        }).ToList();

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Order MapToDomain(OrderEntity entity)
    {
        var lines = entity.Lines.Select(line =>
            new OrderLine(line.ProductId, line.Quantity, new Money(line.UnitPrice)));

        return Order.Rehydrate(
            entity.Id,
            entity.CustomerId,
            Enum.Parse<OrderStatus>(entity.Status),
            entity.CreatedAt,
            lines);
    }

    private static OrderEntity MapToEntity(Order order) =>
        new()
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Lines = order.Lines.Select(line => new OrderLineEntity
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice.Amount
            }).ToList()
        };
}
