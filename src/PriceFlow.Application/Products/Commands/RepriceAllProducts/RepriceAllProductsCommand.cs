using MediatR;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Events;

namespace PriceFlow.Application.Products.Commands.RepriceAllProducts;

/// <summary>Reprices the whole catalog. Returns the number of changed prices.</summary>
public record RepriceAllProductsCommand : IRequest<int>;

public class RepriceAllProductsCommandHandler(
    IProductRepository products,
    ICompetitorPriceClient competitor,
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    TimeProvider time) : IRequestHandler<RepriceAllProductsCommand, int>
{
    public async Task<int> Handle(RepriceAllProductsCommand command, CancellationToken ct)
    {
        var bySku = (await products.GetAllAsync(ct)).ToDictionary(p => p.Sku);
        if (bySku.Count == 0)
            return 0;

        var now = time.GetUtcNow().UtcDateTime;
        var notifications = new List<PriceChangedNotification>();

        // Quotes arrive one by one over a gRPC server stream.
        await foreach (var quote in competitor.GetPricesAsync(bySku.Keys, ct))
        {
            if (!bySku.TryGetValue(quote.Sku, out var product))
                continue;

            var change = product.Reprice(quote.Price, now);
            if (change is null)
                continue;

            products.AddPriceChange(change);
            notifications.Add(PriceChangedNotification.From(product, change));
        }

        await unitOfWork.SaveChangesAsync(ct);

        foreach (var notification in notifications)
            await publisher.Publish(notification, ct);

        return notifications.Count;
    }
}
