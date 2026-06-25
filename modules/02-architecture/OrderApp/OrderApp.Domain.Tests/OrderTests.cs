using OrderApp.Domain.Entities;
using OrderApp.Domain.Exceptions;
using OrderApp.Domain.ValueObjects;

namespace OrderApp.Domain.Tests;

public class OrderTests
{
    private static readonly Guid CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ProductId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void Confirm_WhenOrderHasLines_SetsStatusToConfirmed()
    {
        var order = Order.Create(CustomerId);
        order.AddLine(ProductId, 1, new Money(100m));

        order.Confirm();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.Single(order.DomainEvents);
    }

    [Fact]
    public void Confirm_WhenOrderHasNoLines_ThrowsDomainException()
    {
        var order = Order.Create(CustomerId);

        Assert.Throws<DomainException>(() => order.Confirm());
    }

    [Fact]
    public void AddLine_WhenOrderIsConfirmed_ThrowsDomainException()
    {
        var order = Order.Create(CustomerId);
        order.AddLine(ProductId, 1, new Money(50m));
        order.Confirm();

        Assert.Throws<DomainException>(() => order.AddLine(ProductId, 1, new Money(10m)));
    }
}
