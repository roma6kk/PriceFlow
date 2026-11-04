using PriceFlow.Domain.Common;
using PriceFlow.Domain.Products;

namespace PriceFlow.UnitTests.Domain;

public class ProductTests
{
    private static readonly DateTime Now = new(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void New_product_starts_at_floor_price()
    {
        var product = Product.Create("tea-1001", " Green tea ", 100m, 20m, Now);

        Assert.Equal("TEA-1001", product.Sku);
        Assert.Equal("Green tea", product.Name);
        Assert.Equal(120m, product.Price);
        Assert.Null(product.CompetitorPrice);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(100, -1)]
    [InlineData(100, 501)]
    public void Rejects_invalid_cost_or_margin(decimal cost, decimal margin)
    {
        Assert.Throws<DomainException>(() => Product.Create("TEA-1001", "Green tea", cost, margin, Now));
    }

    [Fact]
    public void Reprice_returns_history_record_when_price_changes()
    {
        var product = Product.Create("TEA-1001", "Green tea", 100m, 20m, Now);

        var change = product.Reprice(competitorPrice: 200m, Now.AddMinutes(1));

        Assert.NotNull(change);
        Assert.Equal(120m, change.OldPrice);
        Assert.Equal(198m, change.NewPrice);
        Assert.Equal(PriceChangeReason.CompetitorPriceChanged, change.Reason);
        Assert.Equal(198m, product.Price);
        Assert.Equal(200m, product.CompetitorPrice);
    }

    [Fact]
    public void Reprice_returns_null_when_price_stays_the_same()
    {
        var product = Product.Create("TEA-1001", "Green tea", 100m, 20m, Now);

        // The competitor is cheaper than our floor, so we stay at 120.
        var change = product.Reprice(competitorPrice: 110m, Now.AddMinutes(1));

        Assert.Null(change);
        Assert.Equal(120m, product.Price);
        Assert.Equal(110m, product.CompetitorPrice);
    }

    [Fact]
    public void Raising_cost_lifts_the_price_to_the_new_floor()
    {
        var product = Product.Create("TEA-1001", "Green tea", 100m, 20m, Now);
        product.Reprice(130m, Now);

        var change = product.ChangeCost(cost: 150m, minMarginPercent: 20m, Now.AddMinutes(1));

        Assert.NotNull(change);
        Assert.Equal(PriceChangeReason.CostChanged, change.Reason);
        Assert.Equal(180m, product.Price);
    }
}
