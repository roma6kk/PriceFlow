using MediatR;
using Microsoft.Extensions.Logging;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Application.Products.Events;

public class PublishPriceChangedHandler(IPriceEventPublisher publisher, ILogger<PublishPriceChangedHandler> logger)
    : INotificationHandler<PriceChangedNotification>
{
    public async Task Handle(PriceChangedNotification notification, CancellationToken ct)
    {
        try
        {
            await publisher.PublishAsync(notification, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish price change for {Sku}", notification.Sku);
        }
    }
}
