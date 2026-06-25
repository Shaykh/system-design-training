using Microsoft.EntityFrameworkCore;
using OrderApp.Application.Abstractions;
using OrderApp.Infrastructure.Persistence;

namespace OrderApp.Infrastructure.Services;

public sealed class EfCustomerService(OrderAppDbContext dbContext) : ICustomerService
{
    public async Task<CustomerSnapshot?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await dbContext.Customers.FindAsync([customerId], cancellationToken);
        return customer is null
            ? null
            : new CustomerSnapshot(customer.Id, customer.Email, customer.CreditLimit);
    }
}
