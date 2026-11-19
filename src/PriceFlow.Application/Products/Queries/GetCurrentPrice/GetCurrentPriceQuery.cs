using MediatR;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products.Queries.GetCurrentPrice;

public record GetCurrentPriceQuery(string Sku) : IRequest<CurrentPriceDto>;

public record CurrentPriceDto(string Sku, decimal Price);

public class GetCurrentPriceQueryHandler(IProductRepository products, IPriceCache cache)
    : IRequestHandler<GetCurrentPriceQuery, CurrentPriceDto>
{
    public async Task<CurrentPriceDto> Handle(GetCurrentPriceQuery query, CancellationToken ct)
    {
        var sku = query.Sku.Trim().ToUpperInvariant();

        var cached = await cache.GetAsync(sku, ct);
        if (cached is not null)
            return new CurrentPriceDto(sku, cached.Value);

        var product = await products.GetBySkuAsync(sku, ct)
            ?? throw new NotFoundException(nameof(Product), sku);

        await cache.SetAsync(sku, product.Price, ct);
        return new CurrentPriceDto(sku, product.Price);
    }
}
