using System.Diagnostics.Tracing;

namespace Pessoto.HubDataPusher.Core
{
    [EventSource(Name = "Pessoto.HubDataPusher.Core.DataPusher")]
    public sealed class DataPusherEventSource : EventSource
    {
        public static readonly DataPusherEventSource Log = new DataPusherEventSource();

        private readonly IncrementingEventCounter eventsSent;
        private readonly IncrementingEventCounter bytesSent;

        public DataPusherEventSource()
        {
            eventsSent = new("events-sent", this);
            bytesSent = new("bytes-sent", this);
        }

        public void EventsSent(int numberOfEvents)
        {
            if (IsEnabled())
            {
                eventsSent.Increment(numberOfEvents);
            }
        }

        public void BytesSent(long sizeInBytes)
        {
            if (IsEnabled())
            {
                bytesSent.Increment(sizeInBytes);
            }
        }

        protected override void Dispose(bool disposing)
        {
            eventsSent?.Dispose();
            bytesSent?.Dispose();
            base.Dispose(disposing);
        }
    }
}
