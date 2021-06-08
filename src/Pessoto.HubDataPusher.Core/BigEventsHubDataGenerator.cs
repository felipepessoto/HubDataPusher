using System;
using System.Text;

namespace Pessoto.HubDataPusher.Core
{
    /// <summary>
    /// Creates events close to the 256KB limit (For Basic Event Hub)
    /// </summary>
    public class BigEventsHubDataGenerator : IHubDataGenerator
    {
        private const double minTemperature = 20;
        private const double minHumidity = 60;

        private static string messageBody;

        private readonly Random r = new();

        static BigEventsHubDataGenerator()
        {
            string sampleMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante erat, ut finibus purus iaculis vel. Sed suscipit tempor ipsum, id ornare arcu. Duis eros velit, dapibus cursus dolor sed, tincidunt egestas ipsum. Mauris non erat diam. Donec aliquet ultrices tincidunt. Vestibulum gravida leo nunc, ac auctor lacus varius sit amet. Quisque ultricies aliquet mauris, et dignissim orci consectetur at.";

            const int maxMessageSize = 250 * 1024;
            StringBuilder sb = new StringBuilder(maxMessageSize);
            int messageBodyLengthLimit = maxMessageSize - sampleMessage.Length;

            while (sb.Length < messageBodyLengthLimit)
            {
                sb.Append(sampleMessage);
            }

            messageBody = sb.ToString();
        }

        public BinaryData GeneratePayload()
        {
            double currentTemperature = minTemperature + NextDoubleRandom() * 15;
            double currentHumidity = minHumidity + NextDoubleRandom() * 20;

            object telemetryDataPoint = new
            {
                createdAt = DateTime.UtcNow,
                deviceId = "EventHubDevice" + NextRandom(1, 300000),
                temperature = currentTemperature,
                humidity = currentHumidity,
                message = messageBody,
            };

            return BinaryData.FromObjectAsJson(telemetryDataPoint);
        }

        private int NextRandom(int minInclusive, int maxExclusive)
        {
            lock (r)
            {
                return r.Next(minInclusive, maxExclusive);
            }
        }

        private double NextDoubleRandom()
        {
            lock (r)
            {
                return r.NextDouble();
            }
        }
    }
}
