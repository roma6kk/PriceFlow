using MediatR;
using Microsoft.Extensions.Options;
using PriceFlow.Application.Products.Commands.RepriceAllProducts;

namespace PriceFlow.Api.Workers;

public class RepricingWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<RepricingOptions> options,
    ILogger<RepricingWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.IntervalSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var changed = await sender.Send(new RepriceAllProductsCommand(), stoppingToken);
                if (changed > 0)
                    logger.LogInformation("Repricing finished, {Changed} prices changed", changed);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Repricing run failed");
            }
        }
    }
}
