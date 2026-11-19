namespace PriceFlow.Domain.Products;

public static class PriceCalculator
{
    public const decimal UndercutPercent = 1m;

    public static decimal FloorPrice(decimal cost, decimal minMarginPercent) =>
        Round(cost * (1 + minMarginPercent / 100));

    public static decimal Calculate(decimal cost, decimal minMarginPercent, decimal? competitorPrice)
    {
        var floor = FloorPrice(cost, minMarginPercent);
        if (competitorPrice is null)
            return floor;

        var target = Round(competitorPrice.Value * (1 - UndercutPercent / 100));
        return Math.Max(target, floor);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
