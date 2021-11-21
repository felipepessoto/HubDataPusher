using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pessoto.HubDataPusher.Core;

namespace Pessoto.HubDataPusher.EventHub.Core;

public class EventHubDataPusher
{
    private readonly EventHubConnection _connection;
    private readonly long? _maximumBatchSize;
    private readonly int _numberOfThreads;

    private readonly IHubDataGenerator _hubDataGenerator;
    private readonly BandwitdhThrottler _bandwitdhThrottler;
    private readonly ILogger<EventHubDataPusher> _logger;

    private static readonly TimeSpan serviceBusyDelay = TimeSpan.FromSeconds(5);
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
        _logger.PusherStartingSender(senderId);

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
            _logger.Exception(ex);
            throw;
        }
    }

    private async Task SendBatch(int senderId, EventHubProducerClient producerClient, CancellationToken cancellationToken)
    {
        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync(new CreateBatchOptions() { MaximumSizeInBytes = _maximumBatchSize }, cancellationToken);

        while (eventBatch.TryAdd(new EventData(_hubDataGenerator.GeneratePayload()))) ;

        _bandwitdhThrottler.Consume(eventBatch.SizeInBytes);

        try
        {
            await producerClient.SendAsync(eventBatch, cancellationToken);
        }
        catch (EventHubsException ex) when (ex.IsTransient)
        {
            _logger.Exception(ex, LogLevel.Warning, "Transient Error");

            if (ex.Reason == EventHubsException.FailureReason.ServiceBusy)
            {
                _logger.PusherServiceBusy(serviceBusyDelay);
                await Task.Delay(serviceBusyDelay, cancellationToken);
            }
        }

        DataPusherEventSource.Log.EventsSent(eventBatch.Count);
        DataPusherEventSource.Log.BytesSent(eventBatch.SizeInBytes);

        _logger.PusherBatchSent(senderId, eventBatch.Count, eventBatch.SizeInBytes);
    }
}
