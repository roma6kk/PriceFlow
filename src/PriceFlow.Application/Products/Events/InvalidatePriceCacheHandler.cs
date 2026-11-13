using MediatR;
using Microsoft.Extensions.Logging;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Application.Products.Events;

public class InvalidatePriceCacheHandler(IPriceCache cache, ILogger<InvalidatePriceCacheHandler> logger)
    : INotificationHandler<PriceChangedNotification>
{
    public async Task Handle(PriceChangedNotification notification, CancellationToken ct)
    {
        try
        {
            await cache.RemoveAsync(notification.Sku, ct);
        }
        catch (Exception ex)
        {
            // The cache is best effort: a stale entry expires by TTL anyway.
            logger.LogWarning(ex, "Failed to invalidate price cache for {Sku}", notification.Sku);
        }
    }
}
