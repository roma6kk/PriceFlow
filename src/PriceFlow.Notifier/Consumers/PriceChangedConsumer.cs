using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using PriceFlow.Contracts.Events;
using PriceFlow.Notifier.Hubs;

namespace PriceFlow.Notifier.Consumers;

/// <summary>Reads price changes from Kafka and pushes them to SignalR clients.</summary>
public class PriceChangedConsumer(
    IConfiguration configuration,
    IHubContext<PricesHub, IPricesClient> hub) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            // Every instance must see every event to push it to its own clients,
            // so each instance gets its own consumer group.
            GroupId = $"notifier-{Environment.MachineName}",
            // A dashboard only cares about fresh prices, no need to replay history.
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(KafkaTopics.PriceChanged);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                var priceChange = JsonSerializer.Deserialize<PriceChangedEvent>(result.Message.Value);
                if (priceChange is null)
                    continue;

                await hub.Clients
                    .Groups(PricesHub.AllGroup, PricesHub.SkuGroup(priceChange.Sku))
                    .PriceChanged(priceChange);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown.
        }
        finally
        {
            consumer.Close();
        }
    }
}
