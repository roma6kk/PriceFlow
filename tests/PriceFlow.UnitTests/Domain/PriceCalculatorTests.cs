using PriceFlow.Domain.Products;

namespace PriceFlow.UnitTests.Domain;

public class PriceCalculatorTests
{
    [Fact]
    public void Undercuts_competitor_by_one_percent()
    {
        var price = PriceCalculator.Calculate(cost: 100m, minMarginPercent: 10m, competitorPrice: 200m);

        Assert.Equal(198m, price);
    }

    [Fact]
    public void Never_goes_below_cost_plus_minimal_margin()
    {
        var price = PriceCalculator.Calculate(cost: 100m, minMarginPercent: 10m, competitorPrice: 105m);

        Assert.Equal(110m, price);
    }

    [Fact]
    public void Uses_floor_price_when_competitor_price_is_unknown()
    {
        var price = PriceCalculator.Calculate(cost: 100m, minMarginPercent: 25m, competitorPrice: null);

        Assert.Equal(125m, price);
    }

    [Theory]
    [InlineData(250.50, 248.00)]
    [InlineData(199.99, 197.99)]
    [InlineData(101.50, 100.49)]
    public void Rounds_to_kopecks_away_from_zero(decimal competitorPrice, decimal expected)
    {
        var price = PriceCalculator.Calculate(cost: 10m, minMarginPercent: 0m, competitorPrice);

        Assert.Equal(expected, price);
    }
}
