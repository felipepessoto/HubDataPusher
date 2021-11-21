using Microsoft.Extensions.Logging;

namespace Pessoto.HubDataPusher.Core;

public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting sender Id: {SenderId}")]
    public static partial void PusherStartingSender(this ILogger logger, int senderId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Sender Id: {SenderId}. A batch of {EventCount} events ({BatchSize} bytes) has been sent.")]
    public static partial void PusherBatchSent(this ILogger logger, int senderId, int eventCount, long batchSize);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "ServiceBusy. Waiting {interval}")]
    public static partial void PusherServiceBusy(this ILogger logger, TimeSpan interval);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "BandwitdhThrottler waiting for: {Interval}")]
    public static partial void BandwitdhThrottlerThrottling(this ILogger logger, TimeSpan interval);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error)]
    public static partial void Exception(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 6, Message = "{Message}")]
    public static partial void Exception(this ILogger logger, Exception exception, LogLevel level, string message);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Worker started")]
    public static partial void WorkerStarted(this ILogger logger);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Worker stopped")]
    public static partial void WorkerStopped(this ILogger logger);
}
