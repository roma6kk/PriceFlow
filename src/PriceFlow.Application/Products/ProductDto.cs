using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products;

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    decimal Cost,
    decimal MinMarginPercent,
    decimal Price,
    decimal? CompetitorPrice,
    DateTime UpdatedAt)
{
    public static ProductDto From(Product p) =>
        new(p.Id, p.Sku, p.Name, p.Cost, p.MinMarginPercent, p.Price, p.CompetitorPrice, p.UpdatedAt);
}
