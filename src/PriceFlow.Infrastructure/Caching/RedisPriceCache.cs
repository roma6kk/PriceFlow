using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Infrastructure.Caching;

public class RedisPriceCache(IDistributedCache cache) : IPriceCache
{
    private static readonly DistributedCacheEntryOptions Options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<decimal?> GetAsync(string sku, CancellationToken ct)
    {
        var value = await cache.GetStringAsync(Key(sku), ct);
        return value is null ? null : decimal.Parse(value, CultureInfo.InvariantCulture);
    }

    public Task SetAsync(string sku, decimal price, CancellationToken ct) =>
        cache.SetStringAsync(Key(sku), price.ToString(CultureInfo.InvariantCulture), Options, ct);

    public Task RemoveAsync(string sku, CancellationToken ct) =>
        cache.RemoveAsync(Key(sku), ct);

    private static string Key(string sku) => $"price:{sku}";
}
