using PriceFlow.Application.Products.Events;

namespace PriceFlow.Application.Common.Interfaces;

public interface IPriceEventPublisher
{
    Task PublishAsync(PriceChangedNotification notification, CancellationToken ct);
}
