namespace PriceFlow.Application.Common.Interfaces;

public interface ICompetitorPriceClient
{
    /// <returns>Competitor price, or null if the competitor does not sell this product.</returns>
    Task<decimal?> GetPriceAsync(string sku, CancellationToken ct);
}
