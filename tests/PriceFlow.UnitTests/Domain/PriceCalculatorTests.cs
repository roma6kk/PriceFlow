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
}
