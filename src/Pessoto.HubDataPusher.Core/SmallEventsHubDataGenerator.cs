namespace Pessoto.HubDataPusher.Core
{
    public class SmallEventsHubDataGenerator : IHubDataGenerator
    {
        const double minTemperature = 20;
        const double minHumidity = 60;

        public BinaryData GeneratePayload()
        {
            double currentTemperature = minTemperature + Random.Shared.NextDouble() * 15;
            double currentHumidity = minHumidity + Random.Shared.NextDouble() * 20;

            object telemetryDataPoint = new
            {
                createdAt = DateTime.UtcNow,
                deviceId = "EventHubDevice" + Random.Shared.Next(1, 300000),
                temperature = currentTemperature,
                humidity = currentHumidity,
            };

            return BinaryData.FromObjectAsJson(telemetryDataPoint);
        }
    }
}
