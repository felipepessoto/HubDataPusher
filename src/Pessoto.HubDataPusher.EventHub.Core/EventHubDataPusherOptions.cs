using System;

namespace Pessoto.HubDataPusher.EventHub.Core
{
    public class EventHubDataPusherOptions
    {
        public string ConnectionString { get; init; } = string.Empty;

        public int BatchSize { get; init; }

        public TimeSpan DelayBetweenBatches { get; init; }

        public int NumberOfThread { get; init; }
    }
}
