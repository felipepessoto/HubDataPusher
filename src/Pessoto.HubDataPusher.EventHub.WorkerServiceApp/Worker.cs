using Pessoto.HubDataPusher.Core;
using Pessoto.HubDataPusher.EventHub.Core;

namespace Pessoto.HubDataPusher.EventHub.WorkerServiceApp;

public class Worker : BackgroundService
{
    private readonly EventHubDataPusher _eventHubDataPusher;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<Worker> _logger;

    public Worker(EventHubDataPusher eventHubDataPusher, IHostApplicationLifetime applicationLifetime, ILogger<Worker> logger)
    {
        _eventHubDataPusher = eventHubDataPusher;
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Task pusherTask = _eventHubDataPusher.Start(stoppingToken);

                _logger.WorkerStarted();
                await pusherTask;
                _logger.WorkerStopped();
            }
        }
        catch (Exception ex)
        {
            _logger.Exception(ex);
            _applicationLifetime.StopApplication();
        }
    }
}
