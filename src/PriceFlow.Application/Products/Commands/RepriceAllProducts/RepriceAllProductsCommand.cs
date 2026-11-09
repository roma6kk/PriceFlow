using MediatR;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Application.Products.Commands.RepriceAllProducts;

/// <summary>Reprices the whole catalog. Returns the number of changed prices.</summary>
public record RepriceAllProductsCommand : IRequest<int>;

public class RepriceAllProductsCommandHandler(
    IProductRepository products,
    ICompetitorPriceClient competitor,
    IUnitOfWork unitOfWork,
    TimeProvider time) : IRequestHandler<RepriceAllProductsCommand, int>
{
    public async Task<int> Handle(RepriceAllProductsCommand command, CancellationToken ct)
    {
        var bySku = (await products.GetAllAsync(ct)).ToDictionary(p => p.Sku);
        if (bySku.Count == 0)
            return 0;

        var now = time.GetUtcNow().UtcDateTime;
        var changed = 0;

        // Quotes arrive one by one over a gRPC server stream.
        await foreach (var quote in competitor.GetPricesAsync(bySku.Keys, ct))
        {
            if (!bySku.TryGetValue(quote.Sku, out var product))
                continue;

            var change = product.Reprice(quote.Price, now);
            if (change is null)
                continue;

            products.AddPriceChange(change);
            changed++;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return changed;
    }
}
