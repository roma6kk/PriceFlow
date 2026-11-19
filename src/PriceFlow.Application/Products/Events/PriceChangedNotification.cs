using MediatR;
using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products.Events;

public record PriceChangedNotification(
    Guid ProductId,
    string Sku,
    string Name,
    decimal OldPrice,
    decimal NewPrice,
    decimal? CompetitorPrice,
    string Reason,
    DateTime ChangedAt) : INotification
{
    public static PriceChangedNotification From(Product product, PriceChange change) =>
        new(product.Id, product.Sku, product.Name, change.OldPrice, change.NewPrice,
            change.CompetitorPrice, change.Reason.ToString(), change.ChangedAt);
}
