using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Events;
using PriceFlow.Contracts.Events;

namespace PriceFlow.Infrastructure.Messaging;

public sealed class KafkaPriceEventPublisher : IPriceEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaPriceEventPublisher(IOptions<KafkaOptions> options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(PriceChangedNotification n, CancellationToken ct)
    {
        var message = new PriceChangedEvent(n.ProductId, n.Sku, n.Name, n.OldPrice, n.NewPrice,
            n.CompetitorPrice, n.Reason, n.ChangedAt);

        await _producer.ProduceAsync(KafkaTopics.PriceChanged, new Message<string, string>
        {
            Key = n.ProductId.ToString(),
            Value = JsonSerializer.Serialize(message)
        }, ct);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
