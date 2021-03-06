namespace Pessoto.HubDataPusher.EventHub.Core
{
    public class EventHubDataPusherOptions
    {
        public string ConnectionString { get; init; } = string.Empty;

        public long? MaximumBatchSize { get; init; }

        public int NumberOfThread { get; init; }
    }
}
