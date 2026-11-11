namespace PriceFlow.Contracts.Events;

/// <summary>Message published to Kafka when a product price changes.</summary>
public record PriceChangedEvent(
    Guid ProductId,
    string Sku,
    string Name,
    decimal OldPrice,
    decimal NewPrice,
    decimal? CompetitorPrice,
    string Reason,
    DateTime ChangedAt);
