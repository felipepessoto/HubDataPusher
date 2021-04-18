namespace Pessoto.HubDataPusher.Core
{
    public class BandwitdhThrottlerOptions
    {
        public bool Enabled { get; init; }

        public float MaxPushRateMBps { get; init; }
    }
}
