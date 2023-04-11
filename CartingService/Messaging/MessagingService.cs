using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class MessagingService
{
    private readonly IItemService _itemService;
    private readonly ILogger _logger;

    public MessagingService(IItemService itemService, ILogger<MessagingService> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    public async Task Consume(string server, string groupId, CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { GroupId = groupId, BootstrapServers = server }).Build();

        consumer.Subscribe(nameof(ItemUpdatedMessage));
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = consumer.Consume(cancellationToken);
            var messageHandled = false;
            while (!messageHandled)
            {
                try
                {
                    if (message.Topic == nameof(ItemUpdatedMessage))
                    {
                        await _itemService.HandleItemUpdated(JsonSerializer.Deserialize<ItemUpdatedMessage>(message.Message.Value)!);
                    }
                    messageHandled = true;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.ToString());
                    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                    if (cancellationToken.IsCancellationRequested) { return; }
                }
            }
        }
        consumer.Close();
    }
}
