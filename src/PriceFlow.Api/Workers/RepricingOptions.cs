namespace PriceFlow.Api.Workers;

public class RepricingOptions
{
    public const string Section = "Repricing";

    public int IntervalSeconds { get; set; } = 15;
}
