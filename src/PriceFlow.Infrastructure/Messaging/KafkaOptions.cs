namespace PriceFlow.Infrastructure.Messaging;

public class KafkaOptions
{
    public const string Section = "Kafka";

    public string BootstrapServers { get; set; } = "localhost:29092";
}
