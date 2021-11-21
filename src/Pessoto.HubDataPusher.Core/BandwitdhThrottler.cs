using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Pessoto.HubDataPusher.Core;

public class BandwitdhThrottler
{
    private const int intervalMilliseconds = 1000;
    private readonly bool _enabled;
    private readonly float _maxPushRateBytesPerMs;
    private readonly ILogger<BandwitdhThrottler> _logger;
    private readonly Stopwatch _watch = new();
    private readonly object sync = new();

    private long _transmittedBytes = 0;

    public BandwitdhThrottler(IOptions<BandwitdhThrottlerOptions> options, ILogger<BandwitdhThrottler> logger)
    {
        _enabled = options.Value.Enabled;
        float maxPushRateBps = options.Value.MaxPushRateMBps * 1024 * 1024;
        _maxPushRateBytesPerMs = (maxPushRateBps / 1000);
        _logger = logger;
    }

    public void Consume(long bytes)
    {
        if (_enabled == false)
        {
            return;
        }

        lock (sync)
        {
            if (_watch.IsRunning == false)
            {
                _watch.Start();
            }

            _transmittedBytes += bytes;
            long elapsedMs = _watch.ElapsedMilliseconds;

            if (elapsedMs > intervalMilliseconds)
            {
                _watch.Restart();
                long expectedTransmitBytes = (long)_maxPushRateBytesPerMs * elapsedMs;
                long balanceBytes = _transmittedBytes - expectedTransmitBytes;

                if (balanceBytes > 0)
                {
                    var sleepInterval = TimeSpan.FromMilliseconds((int)(balanceBytes / _maxPushRateBytesPerMs));

                    _logger.BandwitdhThrottlerThrottling(sleepInterval);
                    
                    Thread.Sleep(sleepInterval);
                }

                _transmittedBytes = 0;
            }
        }
    }
}
