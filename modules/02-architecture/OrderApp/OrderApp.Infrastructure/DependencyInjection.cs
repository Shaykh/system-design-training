using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderApp.Application.Abstractions;
using OrderApp.Infrastructure.Persistence;
using OrderApp.Infrastructure.Persistence.Entities;
using OrderApp.Infrastructure.Services;

namespace OrderApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string databaseName = "OrderAppDb")
    {
        services.AddDbContext<OrderAppDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IProductCatalog, EfProductCatalog>();
        services.AddScoped<ICustomerService, EfCustomerService>();
        services.AddScoped<INotificationService, ConsoleNotificationService>();

        return services;
    }

    public static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderAppDbContext>();

        if (await dbContext.Customers.AnyAsync())
            return;

        var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var productA = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var productB = Guid.Parse("33333333-3333-3333-3333-333333333333");

        dbContext.Customers.Add(new CustomerEntity
        {
            Id = customerId,
            Email = "customer@example.com",
            CreditLimit = 5000m
        });

        dbContext.Products.AddRange(
            new ProductEntity { Id = productA, Name = "Laptop", Price = 999m, Stock = 10 },
            new ProductEntity { Id = productB, Name = "Mouse", Price = 29m, Stock = 100 });

        await dbContext.SaveChangesAsync();
    }
}
