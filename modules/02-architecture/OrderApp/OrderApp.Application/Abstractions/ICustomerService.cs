namespace OrderApp.Application.Abstractions;

public interface ICustomerService
{
    Task<CustomerSnapshot?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
}

public sealed record CustomerSnapshot(Guid Id, string Email, decimal CreditLimit);
