﻿using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class MessagingService : IMessagingService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public MessagingService(IMessageRepository messageRepository, IUnitOfWork unitOfWork, ILogger<MessagingService> logger)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task Save(BaseMessage message)
    {
        message.TraceId = Activity.Current?.TraceId.ToString();
        message.SpanId = Activity.Current?.SpanId.ToString();
        return _messageRepository.Create(message.GetType().Name, JsonSerializer.Serialize(message));
    }

    public async Task Produce(string server, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = server }).Build();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var messages = (await _messageRepository.GetAll()).ToArray();
                if (messages.Any())
                {
                    await _unitOfWork.Start();
                    foreach (var message in messages)
                    {
                        await producer.ProduceAsync(message.Name, new() { Value = message.Contents }, cancellationToken);
                        await _messageRepository.Delete(message.Id);
                    }
                    producer.Flush(cancellationToken);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken); // avoid spamming Kafka in case of an error
                if (cancellationToken.IsCancellationRequested) { return; }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // wait for new messages
        }
    }
}
