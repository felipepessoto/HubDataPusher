namespace Pessoto.HubDataPusher.Core
{
    public class SmallEventsHubDataGenerator : IHubDataGenerator
    {
        const double minTemperature = 20;
        const double minHumidity = 60;

        private readonly Random r = new();

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
            };

            return BinaryData.FromObjectAsJson(telemetryDataPoint);
        }

        private int NextRandom(int minInclusive, int maxExclusive)
        {
            return Random.Shared.Next(minInclusive, maxExclusive);
        }

        private double NextDoubleRandom()
        {
            return Random.Shared.NextDouble();
        }
    }
}
