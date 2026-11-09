namespace PriceFlow.Application.Common.Interfaces;

public record CompetitorQuote(string Sku, decimal Price);

public interface ICompetitorPriceClient
{
    /// <returns>Competitor price, or null if the competitor does not sell this product.</returns>
    Task<decimal?> GetPriceAsync(string sku, CancellationToken ct);

    IAsyncEnumerable<CompetitorQuote> GetPricesAsync(IReadOnlyCollection<string> skus, CancellationToken ct);
}
