public record MessagingServiceConfiguration(string Server, string Group);

public class MessagingHostedService : IHostedService
{
    private readonly MessagingService _messagingService;
    private readonly MessagingServiceConfiguration _configuration;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ILogger _logger;

    public MessagingHostedService(MessagingService messagingService, MessagingServiceConfiguration configuration, ILogger<MessagingHostedService> logger)
    {
        _messagingService = messagingService;
        _configuration = configuration;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken _cancellationToken)
    {
        Task.Run(async () =>
        {
            try
            {
                await _messagingService.Consume(_configuration.Server, _configuration.Group, _cancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
            }
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken _cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}
