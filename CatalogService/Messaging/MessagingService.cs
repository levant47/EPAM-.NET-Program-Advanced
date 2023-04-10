using System.Text.Json;
using Confluent.Kafka;

public class MessagingService : IMessagingService, IDisposable
{
    private readonly IProducer<Null, string> _producer;

    public MessagingService(string server) => _producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = server }).Build();

    public void Send(object message) => _producer.Produce(message.GetType().Name, new() { Value = JsonSerializer.Serialize(message) });

    public void Dispose()
    {
        _producer.Flush(); // force send all the queued up messages, Dispose doesn't do it by itself for some reason
        _producer.Dispose();
    }
}
