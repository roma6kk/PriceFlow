using Microsoft.AspNetCore.SignalR;

namespace PriceFlow.Notifier.Hubs;

public class PricesHub : Hub<IPricesClient>
{
    public const string AllGroup = "all";

    public static string SkuGroup(string sku) => $"sku:{sku.ToUpperInvariant()}";

    public Task SubscribeAll() =>
        Groups.AddToGroupAsync(Context.ConnectionId, AllGroup);

    public Task Subscribe(string sku) =>
        Groups.AddToGroupAsync(Context.ConnectionId, SkuGroup(sku));

    public Task Unsubscribe(string sku) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, SkuGroup(sku));
}
