using System;

namespace Pessoto.HubDataPusher.Core
{
    public class StaticDataHubDataGenerator : IHubDataGenerator
    {
        const double minTemperature = 20;
        const double minHumidity = 60;
        const string stringData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam laoreet aliquam elit, sit amet mattis velit imperdiet eget. Quisque erat nibh, efficitur condimentum eros ut, pharetra posuere felis. Etiam leo dui, venenatis vitae finibus et, accumsan quis velit. Curabitur non dapibus sapien. Sed commodo velit eget ex luctus malesuada. Phasellus dignissim orci quis turpis eleifend, at molestie nibh laoreet. Phasellus tincidunt iaculis ligula, in vestibulum ante vestibulum ut. Duis ultrices sollicitudin erat, eget tempor eros. Sed nisi augue, accumsan et scelerisque a, faucibus nec ex. Vestibulum tempor rutrum massa in condimentum. In sit amet finibus arcu. Phasellus in commodo sem. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc eu semper diam.";

        private readonly BinaryData binaryData;

        public StaticDataHubDataGenerator()
        {
            Random r = new();
            double currentTemperature = minTemperature + r.NextDouble() * 15;
            double currentHumidity = minHumidity + r.NextDouble() * 20;

            object telemetryDataPoint = new
            {
                createdAt = DateTime.UtcNow,
                deviceId = "EventHubDevice",
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

            binaryData = BinaryData.FromObjectAsJson(telemetryDataPoint);
        }

        public BinaryData GeneratePayload()
        {
            return binaryData;
        }
    }
}
