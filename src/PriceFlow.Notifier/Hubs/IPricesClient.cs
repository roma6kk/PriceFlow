using PriceFlow.Contracts.Events;

namespace PriceFlow.Notifier.Hubs;

public interface IPricesClient
{
    Task PriceChanged(PriceChangedEvent priceChange);
}
