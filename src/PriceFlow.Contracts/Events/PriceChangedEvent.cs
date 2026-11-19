namespace PriceFlow.Contracts.Events;

public record PriceChangedEvent(
    Guid ProductId,
    string Sku,
    string Name,
    decimal OldPrice,
    decimal NewPrice,
    decimal? CompetitorPrice,
    string Reason,
    DateTime ChangedAt);
