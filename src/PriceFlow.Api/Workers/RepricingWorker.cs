using MediatR;
using Microsoft.Extensions.Options;
using PriceFlow.Application.Products.Commands.RepriceAllProducts;

namespace PriceFlow.Api.Workers;

/// <summary>Reprices the whole catalog on a timer.</summary>
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
                // The worker is a singleton, but handlers need a scoped DbContext,
                // so every run gets its own scope, just like an HTTP request does.
                await using var scope = scopeFactory.CreateAsyncScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var changed = await sender.Send(new RepriceAllProductsCommand(), stoppingToken);
                if (changed > 0)
                    logger.LogInformation("Repricing finished, {Changed} prices changed", changed);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // E.g. the competitor service is down. Skip this run and try again on the next tick.
                logger.LogError(ex, "Repricing run failed");
            }
        }
    }
}
