using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pessoto.HubDataPusher.EventHub.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Pessoto.HubDataPusher.EventHub.WorkerServiceApp
{
    public class Worker : BackgroundService
    {
        private readonly EventHubDataPusher _eventHubDataPusher;
        private readonly ILogger<Worker> _logger;

        public Worker(EventHubDataPusher eventHubDataPusher, ILogger<Worker> logger)
        {
            _eventHubDataPusher = eventHubDataPusher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Task pusherTask = _eventHubDataPusher.Start(stoppingToken);

                _logger.LogInformation("Started");
                await pusherTask;
                _logger.LogInformation("EventHubDataPusher stopped");
            }
        }
    }
}
