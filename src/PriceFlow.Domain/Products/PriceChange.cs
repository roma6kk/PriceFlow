namespace PriceFlow.Domain.Products;

/// <summary>One row of price history.</summary>
public class PriceChange
{
    private PriceChange() { }

    internal PriceChange(Guid productId, decimal oldPrice, decimal newPrice,
        decimal? competitorPrice, PriceChangeReason reason, DateTime changedAt)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        CompetitorPrice = competitorPrice;
        Reason = reason;
        ChangedAt = changedAt;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal OldPrice { get; private set; }
    public decimal NewPrice { get; private set; }
    public decimal? CompetitorPrice { get; private set; }
    public PriceChangeReason Reason { get; private set; }
    public DateTime ChangedAt { get; private set; }
}
