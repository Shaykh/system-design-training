using Microsoft.Extensions.Logging;
using OrderApp.Application.Abstractions;

namespace OrderApp.Infrastructure.Services;

/// <summary>
/// Remplace SMTP en environnement de formation — les emails sont loggés en console.
/// </summary>
public sealed class ConsoleNotificationService(ILogger<ConsoleNotificationService> logger) : INotificationService
{
    public Task SendOrderConfirmationAsync(
        Guid orderId,
        string recipientEmail,
        decimal totalAmount,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Order confirmation email sent to {Email} for order {OrderId} (total: {Total:F2} EUR)",
            recipientEmail,
            orderId,
            totalAmount);

        return Task.CompletedTask;
    }
}
