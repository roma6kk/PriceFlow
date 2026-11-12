using PriceFlow.Contracts.Events;

namespace PriceFlow.Notifier.Hubs;

/// <summary>Methods the server can call on connected clients.</summary>
public interface IPricesClient
{
    Task PriceChanged(PriceChangedEvent priceChange);
}
