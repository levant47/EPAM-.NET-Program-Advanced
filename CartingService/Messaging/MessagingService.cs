using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class MessagingService
{
    private static readonly ActivitySource _activitySource = new("Carting Service");

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
                            var messagePayload = JsonSerializer.Deserialize(message.Message.Value, messageType)!;
                            var baseMessage = (BaseMessage)messagePayload;
                            using var activity = _activitySource.StartActivity(
                                "Handling message",
                                ActivityKind.Consumer,
                                new ActivityContext(
                                    ActivityTraceId.CreateFromString(baseMessage.TraceId),
                                    ActivitySpanId.CreateFromString(baseMessage.SpanId),
                                    ActivityTraceFlags.Recorded
                                )
                            );
                            await (Task)handlerMethod.Invoke(handler, new[] { messagePayload })!;
                            consumer.Commit();
                            activity?.Stop();
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
