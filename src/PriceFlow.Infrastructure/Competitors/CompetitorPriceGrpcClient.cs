using System.Runtime.CompilerServices;
using Grpc.Core;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Contracts.Grpc;

namespace PriceFlow.Infrastructure.Competitors;

public class CompetitorPriceGrpcClient(CompetitorPrices.CompetitorPricesClient client) : ICompetitorPriceClient
{
    private static readonly TimeSpan UnaryTimeout = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan StreamTimeout = TimeSpan.FromSeconds(10);

    public async Task<decimal?> GetPriceAsync(string sku, CancellationToken ct)
    {
        try
        {
            var reply = await client.GetPriceAsync(
                new GetPriceRequest { Sku = sku },
                deadline: DateTime.UtcNow.Add(UnaryTimeout),
                cancellationToken: ct);

            return FromKopecks(reply.PriceKopecks);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async IAsyncEnumerable<CompetitorQuote> GetPricesAsync(
        IReadOnlyCollection<string> skus, [EnumeratorCancellation] CancellationToken ct)
    {
        var request = new GetPricesRequest();
        request.Skus.AddRange(skus);

        using var call = client.GetPrices(request,
            deadline: DateTime.UtcNow.Add(StreamTimeout),
            cancellationToken: ct);

        await foreach (var reply in call.ResponseStream.ReadAllAsync(ct))
            yield return new CompetitorQuote(reply.Sku, FromKopecks(reply.PriceKopecks));
    }

    private static decimal FromKopecks(long kopecks) => kopecks / 100m;
}
