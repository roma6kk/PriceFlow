using MediatR;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

public class GetProductsQueryHandler(IProductRepository products)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery query, CancellationToken ct)
    {
        var all = await products.GetAllAsync(ct);
        return all.Select(ProductDto.From).ToList();
    }
}
