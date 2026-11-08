using Grpc.Core;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Contracts.Grpc;

namespace PriceFlow.Infrastructure.Competitors;

public class CompetitorPriceGrpcClient(CompetitorPrices.CompetitorPricesClient client) : ICompetitorPriceClient
{
    private static readonly TimeSpan UnaryTimeout = TimeSpan.FromSeconds(2);

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

    private static decimal FromKopecks(long kopecks) => kopecks / 100m;
}
