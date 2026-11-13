namespace PriceFlow.Application.Common.Interfaces;

public interface IPriceCache
{
    Task<decimal?> GetAsync(string sku, CancellationToken ct);
    Task SetAsync(string sku, decimal price, CancellationToken ct);
    Task RemoveAsync(string sku, CancellationToken ct);
}
