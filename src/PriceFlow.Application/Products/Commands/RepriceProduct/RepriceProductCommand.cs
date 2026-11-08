using MediatR;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products.Commands.RepriceProduct;

public record RepriceProductCommand(Guid ProductId) : IRequest<RepriceResult>;

public record RepriceResult(bool PriceChanged, decimal Price, decimal? CompetitorPrice);

public class RepriceProductCommandHandler(
    IProductRepository products,
    ICompetitorPriceClient competitor,
    IUnitOfWork unitOfWork,
    TimeProvider time) : IRequestHandler<RepriceProductCommand, RepriceResult>
{
    public async Task<RepriceResult> Handle(RepriceProductCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), command.ProductId);

        var competitorPrice = await competitor.GetPriceAsync(product.Sku, ct);
        if (competitorPrice is null)
            return new RepriceResult(false, product.Price, product.CompetitorPrice);

        var change = product.Reprice(competitorPrice.Value, time.GetUtcNow().UtcDateTime);
        if (change is not null)
            products.AddPriceChange(change);

        await unitOfWork.SaveChangesAsync(ct);

        return new RepriceResult(change is not null, product.Price, product.CompetitorPrice);
    }
}
