using System;

namespace Pessoto.HubDataPusher.Core
{
    public class SampleHubDataGenerator : IHubDataGenerator
    {
        const double minTemperature = 20;
        const double minHumidity = 60;
        const string stringData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam laoreet aliquam elit, sit amet mattis velit imperdiet eget. Quisque erat nibh, efficitur condimentum eros ut, pharetra posuere felis. Etiam leo dui, venenatis vitae finibus et, accumsan quis velit. Curabitur non dapibus sapien. Sed commodo velit eget ex luctus malesuada. Phasellus dignissim orci quis turpis eleifend, at molestie nibh laoreet. Phasellus tincidunt iaculis ligula, in vestibulum ante vestibulum ut. Duis ultrices sollicitudin erat, eget tempor eros. Sed nisi augue, accumsan et scelerisque a, faucibus nec ex. Vestibulum tempor rutrum massa in condimentum. In sit amet finibus arcu. Phasellus in commodo sem. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc eu semper diam.";

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
                body1 = stringData,
                body2 = stringData,
                body3 = stringData,
                body4 = stringData,
                body5 = stringData,
                body6 = stringData,
                body7 = stringData,
                body8 = stringData,
                body9 = stringData,
                body10 = stringData
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
