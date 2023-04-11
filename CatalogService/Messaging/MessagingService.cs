using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class MessagingService : IMessagingService
{
    private static readonly ConcurrentQueue<object> _messages = new();

    private readonly ILogger _logger;

    public MessagingService(ILogger<MessagingService> logger) => _logger = logger;

    public void Send(object message) => _messages.Enqueue(message);

    public async Task Produce(string server, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = server }).Build();

        while (!cancellationToken.IsCancellationRequested)
        {
            var anyMessagesSent = false;
            while (_messages.TryDequeue(out var message))
            {
                var messageSent = false;
                while (!messageSent)
                {
                    try
                    {
                        await producer.ProduceAsync(message.GetType().Name, new() { Value = JsonSerializer.Serialize(message) }, cancellationToken);
                        messageSent = true;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.ToString());
                        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken); // avoid spamming Kafka in case of an error
                        if (cancellationToken.IsCancellationRequested) { return; }
                    }
                }
                anyMessagesSent = true;
            }
            if (anyMessagesSent) { producer.Flush(cancellationToken); }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // wait for new messages
        }
    }
}
