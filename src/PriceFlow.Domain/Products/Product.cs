using PriceFlow.Domain.Common;

namespace PriceFlow.Domain.Products;

public class Product
{
    private Product() { }

    public Guid Id { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal Cost { get; private set; }
    public decimal MinMarginPercent { get; private set; }
    public decimal Price { get; private set; }
    public decimal? CompetitorPrice { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public uint Version { get; private set; }

    public static Product Create(string sku, string name, decimal cost, decimal minMarginPercent, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");
        EnsureValidCost(cost, minMarginPercent);

        return new Product
        {
            Id = Guid.NewGuid(),
            Sku = sku.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Cost = cost,
            MinMarginPercent = minMarginPercent,
            Price = PriceCalculator.FloorPrice(cost, minMarginPercent),
            UpdatedAt = now
        };
    }

    public PriceChange? Reprice(decimal competitorPrice, DateTime now)
    {
        if (competitorPrice <= 0)
            throw new DomainException("Competitor price must be positive.");

        CompetitorPrice = competitorPrice;
        return ApplyPrice(PriceChangeReason.CompetitorPriceChanged, now);
    }

    public PriceChange? ChangeCost(decimal cost, decimal minMarginPercent, DateTime now)
    {
        EnsureValidCost(cost, minMarginPercent);

        Cost = cost;
        MinMarginPercent = minMarginPercent;
        return ApplyPrice(PriceChangeReason.CostChanged, now);
    }

    private PriceChange? ApplyPrice(PriceChangeReason reason, DateTime now)
    {
        var newPrice = PriceCalculator.Calculate(Cost, MinMarginPercent, CompetitorPrice);
        UpdatedAt = now;
        if (newPrice == Price)
            return null;

        var change = new PriceChange(Id, Price, newPrice, CompetitorPrice, reason, now);
        Price = newPrice;
        return change;
    }

    private static void EnsureValidCost(decimal cost, decimal minMarginPercent)
    {
        if (cost <= 0)
            throw new DomainException("Cost must be positive.");
        if (minMarginPercent is < 0 or > 500)
            throw new DomainException("Minimal margin must be between 0 and 500 percent.");
    }
}
