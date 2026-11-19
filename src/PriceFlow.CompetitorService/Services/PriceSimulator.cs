namespace PriceFlow.CompetitorService.Services;

public class PriceSimulator(TimeProvider time)
{
    private const int StepSeconds = 30;

    public decimal? GetPrice(string sku)
    {
        if (sku.StartsWith("EXCL-", StringComparison.OrdinalIgnoreCase))
            return null;

        var hash = StableHash(sku.ToUpperInvariant());
        var basePrice = 200 + hash % 1800;
        var phase = hash % 360 * Math.PI / 180;

        var step = time.GetUtcNow().ToUnixTimeSeconds() / StepSeconds;
        var drift = Math.Sin(step / 6.0 + phase) * 0.05;

        return Math.Round(basePrice * (decimal)(1 + drift), 2);
    }

    private static uint StableHash(string value)
    {
        var hash = 2166136261;
        foreach (var c in value)
            hash = (hash ^ c) * 16777619;
        return hash;
    }
}
