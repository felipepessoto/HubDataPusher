using Microsoft.Extensions.Options;
using System;

namespace Pessoto.HubDataPusher.Core
{
    public class StaticDataHubDataGenerator : IHubDataGenerator
    {
        private readonly BinaryData binaryData;

        public StaticDataHubDataGenerator(IOptions<StaticDataHubDataGeneratorOptions> options)
        {
            binaryData = BinaryData.FromString(options.Value.Payload);
        }

        public BinaryData GeneratePayload()
        {
            return binaryData;
        }
    }
}
