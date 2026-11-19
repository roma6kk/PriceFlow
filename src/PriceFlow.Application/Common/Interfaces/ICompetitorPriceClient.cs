namespace PriceFlow.Application.Common.Interfaces;

public record CompetitorQuote(string Sku, decimal Price);

public interface ICompetitorPriceClient
{
    Task<decimal?> GetPriceAsync(string sku, CancellationToken ct);

    IAsyncEnumerable<CompetitorQuote> GetPricesAsync(IReadOnlyCollection<string> skus, CancellationToken ct);
}
