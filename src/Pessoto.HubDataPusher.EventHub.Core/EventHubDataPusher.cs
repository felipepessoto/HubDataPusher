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
        private readonly long? _maximumBatchSize;
        private readonly int _numberOfThreads;

        private readonly IHubDataGenerator _hubDataGenerator;
        private readonly BandwitdhThrottler _bandwitdhThrottler;
        private readonly ILogger<EventHubDataPusher> _logger;

        private static int senderNumber = 0;

        public EventHubDataPusher(IOptions<EventHubDataPusherOptions> options, IHubDataGenerator hubDataGenerator, BandwitdhThrottler bandwitdhThrottler, ILogger<EventHubDataPusher> logger)
        {
            _connection = new EventHubConnection(options.Value.ConnectionString);
            _maximumBatchSize = options.Value.MaximumBatchSize;
            _numberOfThreads = options.Value.NumberOfThread;

            _hubDataGenerator = hubDataGenerator;
            _bandwitdhThrottler = bandwitdhThrottler;
            _logger = logger;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            Task[] tasks = new Task[_numberOfThreads];

            for (int i = 0; i < _numberOfThreads; i++)
            {
                tasks[i] = Task.Factory.StartNew(async () => { await SendBatchLoop(Interlocked.Increment(ref senderNumber), cancellationToken); }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) { }
        }

        private async Task SendBatchLoop(int senderId, CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Starting sender Id: {senderId}");
            }

            try
            {
                await using EventHubProducerClient producerClient = new(_connection);
                while (cancellationToken.IsCancellationRequested == false)
                {
                    await SendBatch(senderId, producerClient, cancellationToken);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                throw;
            }
        }

        private async Task SendBatch(int senderId, EventHubProducerClient producerClient, CancellationToken cancellationToken)
        {
            using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync(new CreateBatchOptions() { MaximumSizeInBytes = _maximumBatchSize }, cancellationToken))
            {
                while (eventBatch.TryAdd(new EventData(_hubDataGenerator.GeneratePayload()))) ;

                _bandwitdhThrottler.Consume(eventBatch.SizeInBytes);

                try
                {
                    await producerClient.SendAsync(eventBatch, cancellationToken);
                }
                catch(EventHubsException ex) when (ex.IsTransient)
                {
                    _logger.LogWarning("Transient Error:");
                    _logger.LogWarning(ex.ToString());

                    if(ex.Reason == EventHubsException.FailureReason.ServiceBusy)
                    {
                        _logger.LogWarning("ServiceBusy. Waiting 5 seconds");
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    }
                }

                DataPusherEventSource.Log.EventsSent(eventBatch.Count);
                DataPusherEventSource.Log.BytesSent(eventBatch.SizeInBytes);

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Sender Id: {senderId}. A batch of {eventBatch.Count} events ({eventBatch.SizeInBytes} bytes) has been published.");
                }
            }
        }
    }
}
