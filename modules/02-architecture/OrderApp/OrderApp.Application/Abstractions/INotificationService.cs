namespace OrderApp.Application.Abstractions;

public interface INotificationService
{
    Task SendOrderConfirmationAsync(Guid orderId, string recipientEmail, decimal totalAmount, CancellationToken cancellationToken = default);
}
