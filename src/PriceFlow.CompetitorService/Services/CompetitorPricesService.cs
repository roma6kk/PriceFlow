using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PriceFlow.Contracts.Grpc;

namespace PriceFlow.CompetitorService.Services;

public class CompetitorPricesService(PriceSimulator simulator, TimeProvider time)
    : CompetitorPrices.CompetitorPricesBase
{
    public override Task<PriceReply> GetPrice(GetPriceRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Sku))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "SKU is required."));

        var price = simulator.GetPrice(request.Sku)
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"No competitor price for '{request.Sku}'."));

        return Task.FromResult(ToReply(request.Sku, price));
    }

    public override async Task GetPrices(GetPricesRequest request,
        IServerStreamWriter<PriceReply> responseStream, ServerCallContext context)
    {
        foreach (var sku in request.Skus)
        {
            if (simulator.GetPrice(sku) is { } price)
                await responseStream.WriteAsync(ToReply(sku, price), context.CancellationToken);
        }
    }

    private PriceReply ToReply(string sku, decimal price) => new()
    {
        Sku = sku,
        PriceKopecks = (long)(price * 100),
        ObservedAt = Timestamp.FromDateTimeOffset(time.GetUtcNow())
    };
}
