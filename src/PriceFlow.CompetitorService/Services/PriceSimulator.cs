namespace PriceFlow.CompetitorService.Services;

/// <summary>
/// Stands in for a real competitor price feed (a parser or a partner API).
/// Every SKU gets a stable base price that slowly drifts up and down within 5%.
/// </summary>
public class PriceSimulator(TimeProvider time)
{
    private const int StepSeconds = 30;

    /// <returns>Price in rubles, or null if the competitor does not sell the product.</returns>
    public decimal? GetPrice(string sku)
    {
        // Own-brand products are not sold anywhere else.
        if (sku.StartsWith("EXCL-", StringComparison.OrdinalIgnoreCase))
            return null;

        var hash = StableHash(sku.ToUpperInvariant());
        var basePrice = 200 + hash % 1800;
        var phase = hash % 360 * Math.PI / 180;

        // The price moves in steps, as if the competitor updated it every 30 seconds.
        var step = time.GetUtcNow().ToUnixTimeSeconds() / StepSeconds;
        var drift = Math.Sin(step / 6.0 + phase) * 0.05;

        return Math.Round(basePrice * (decimal)(1 + drift), 2);
    }

    // string.GetHashCode() is randomized per process, so prices would jump after every restart.
    private static uint StableHash(string value)
    {
        var hash = 2166136261;
        foreach (var c in value)
            hash = (hash ^ c) * 16777619;
        return hash;
    }
}
