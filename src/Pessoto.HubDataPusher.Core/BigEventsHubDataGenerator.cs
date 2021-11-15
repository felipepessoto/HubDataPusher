using Microsoft.Extensions.Options;
using System.Text;

namespace Pessoto.HubDataPusher.Core
{
    /// <summary>
    /// Creates events close to the 256KB limit (For Basic Event Hub)
    /// </summary>
    public class BigEventsHubDataGenerator : IHubDataGenerator
    {
        private const int maxMessageSize = 250 * 1024;

        private readonly string[] IdValues;
        private readonly string messageBody;
        private readonly Random r = new();

        public BigEventsHubDataGenerator(IOptions<BigEventsHubDataGeneratorOptions> options)
        {
            IdValues = new string[options.Value.NumberOfIds];

            for (int i = 0; i < IdValues.Length; i++)
            {
                IdValues[i] = $"{options.Value.IdPropertyValuePrefix}_{i + 1:0000000000}";//Pre-allocates all the string and avoid allocating it every time a payload is generated
            }

            messageBody = GenerateBigString();
        }

        public BinaryData GeneratePayload()
        {
            object telemetryDataPoint = new
            {
                deviceId = IdValues[NextRandom(0, IdValues.Length)],
                message = messageBody,
            };

            return BinaryData.FromObjectAsJson(telemetryDataPoint);
        }

        private static string GenerateBigString()
        {
            string sampleMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam sagittis ante erat, ut finibus purus iaculis vel. Sed suscipit tempor ipsum, id ornare arcu. Duis eros velit, dapibus cursus dolor sed, tincidunt egestas ipsum. Mauris non erat diam. Donec aliquet ultrices tincidunt. Vestibulum gravida leo nunc, ac auctor lacus varius sit amet. Quisque ultricies aliquet mauris, et dignissim orci consectetur at.";

            StringBuilder sb = new StringBuilder(maxMessageSize);
            int messageBodyLengthLimit = maxMessageSize - sampleMessage.Length;

            while (sb.Length < messageBodyLengthLimit)
            {
                sb.Append(sampleMessage);
            }

            return sb.ToString();
        }

        private int NextRandom(int minInclusive, int maxExclusive)
        {
            lock (r)
            {
                return r.Next(minInclusive, maxExclusive);
            }
        }
    }
}
