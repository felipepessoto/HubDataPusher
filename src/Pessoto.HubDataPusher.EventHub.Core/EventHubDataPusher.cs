using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pessoto.HubDataPusher.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pessoto.HubDataPusher.EventHub.Core
{
    public class EventHubDataPusher
    {
        private readonly EventHubConnection _connection;
        private readonly IHubDataGenerator _hubDataGenerator;
        private readonly ILogger<EventHubDataPusher> _logger;
        private readonly int _batchSize;
        private readonly TimeSpan _delayBetweenBatches;
        private readonly int _numberOfThreads;

        public EventHubDataPusher(IOptions<EventHubDataPusherOptions> options, IHubDataGenerator hubDataGenerator, ILogger<EventHubDataPusher> logger)
        {
            _connection = new EventHubConnection(options.Value.ConnectionString);
            _hubDataGenerator = hubDataGenerator;
            _logger = logger;
            _batchSize = options.Value.BatchSize;
            _delayBetweenBatches = options.Value.DelayBetweenBatches;
            _numberOfThreads = options.Value.NumberOfThread;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            Task[] tasks = new Task[_numberOfThreads];

            for (int i = 0; i < _numberOfThreads; i++)
            {
                tasks[i] = Task.Factory.StartNew(async () => { await SendBatchLoop(cancellationToken); }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) { }
        }

        private async Task SendBatchLoop(CancellationToken cancellationToken)
        {
            await using (EventHubProducerClient producerClient = new EventHubProducerClient(_connection))
            {
                while (true)
                {
                    await SendBatch(producerClient, cancellationToken);
                    if (_delayBetweenBatches != TimeSpan.Zero)
                    {
                        await Task.Delay(_delayBetweenBatches, cancellationToken);
                    }
                }
            }
        }

        private async Task SendBatch(EventHubProducerClient producerClient, CancellationToken cancellationToken)
        {
            using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync(cancellationToken))
            {
                for (int i = 0; i < _batchSize; i++)
                {
                    eventBatch.TryAdd(new EventData(_hubDataGenerator.GeneratePayload()));
                }
                await producerClient.SendAsync(eventBatch, cancellationToken);
            }

            _logger.LogInformation($"Thread: {Thread.CurrentThread.ManagedThreadId}. {DateTime.Now} - A batch of {_batchSize} events has been published.");
        }
    }
}
