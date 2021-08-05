using System;
using System.Collections.Generic;

namespace Pessoto.HubDataPusher.Core
{
    public class DynamicSchemaHubDataGenerator : IHubDataGenerator
    {
        private const double minTemperature = 20;
        private const int NumberOfAdditionalColumns = 1100;
        private readonly Random r = new();

        public BinaryData GeneratePayload()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["deviceId"] = "EventHubDevice" + NextRandom(1, 300000);

            for (int i = 0; i < NumberOfAdditionalColumns; i++)
            {
                data["temperature" + i.ToString("0000")] = minTemperature + NextDoubleRandom() * 15;
            }

            return BinaryData.FromObjectAsJson(data);
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
