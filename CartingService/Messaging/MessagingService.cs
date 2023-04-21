using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class MessagingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public MessagingService(IServiceProvider serviceProvider, ILogger<MessagingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Consume(string server, string groupId, CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { GroupId = groupId, BootstrapServers = server }).Build();

        consumer.Subscribe(nameof(ItemUpdatedMessage));
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = consumer.Consume(cancellationToken);
            // resolve the handler for this message and invoke it
            var sharedBllAssembly = typeof(MessageAssemblyMarker).Assembly;
            var messageTypeName = message.Topic;
            var messageType = sharedBllAssembly.GetTypes().FirstOrDefault(type => type.Name == messageTypeName);
            if (messageType != null)
            {
                var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);
                var handler = _serviceProvider.GetService(handlerType);
                if (handler != null)
                {
                    var handlerMethod = handlerType.GetMethod(nameof(IMessageHandler<object>.Handle))!;
                    var messageHandled = false;
                    while (!messageHandled)
                    {
                        try
                        {
                            await (Task)handlerMethod.Invoke(handler, new[] { JsonSerializer.Deserialize(message.Message.Value, messageType) })!;
                            consumer.Commit();
                            messageHandled = true;
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception.ToString());
                            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Handler for message type {messageTypeName} was not found");
                }
            }
            else
            {
                _logger.LogWarning($"Message type {messageTypeName} was not recognized");
            }
        }
        consumer.Close();
    }
}
