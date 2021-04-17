using System;

namespace Pessoto.HubDataPusher.Core
{
    public interface IHubDataGenerator
    {
        public BinaryData GeneratePayload();
    }
}
