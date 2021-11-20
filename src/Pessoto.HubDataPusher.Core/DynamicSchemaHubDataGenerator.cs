using Microsoft.Extensions.Options;

namespace Pessoto.HubDataPusher.Core
{
    public class DynamicSchemaHubDataGenerator : IHubDataGenerator
    {
        private const double minTemperature = 20;
        private readonly string[] IdValues;
        private readonly string[] PropertyValueNames;

        public string IdPropertyName { get; }

        public DynamicSchemaHubDataGenerator(IOptions<DynamicSchemaHubDataGeneratorOptions> options)
        {
            IdPropertyName = options.Value.IdPropertyName;

            IdValues = new string[options.Value.NumberOfIds];
            for (int i = 0; i < IdValues.Length; i++)
            {
                IdValues[i] = $"{options.Value.IdPropertyValuePrefix}_{i + 1:0000000000}";//Pre-allocates all the string and avoid allocating it every time a payload is generated
            }

            PropertyValueNames = new string[options.Value.NumberOfProperties];
            for (int i = 0; i < PropertyValueNames.Length; i++)
            {
                PropertyValueNames[i] = $"{options.Value.ValuePropertyName}_{i + 1:0000000000}";//Pre-allocates all the string and avoid allocating it every time a payload is generated
            }
        }

        public BinaryData GeneratePayload()
        {
            Dictionary<string, object> data = new(PropertyValueNames.Length + 1)
            {
                [IdPropertyName] = IdValues[Random.Shared.Next(0, IdValues.Length)]
            };

            for (int i = 0; i < PropertyValueNames.Length; i++)
            {
                data[PropertyValueNames[i]] = minTemperature + Random.Shared.NextDouble() * 15;
            }

            return BinaryData.FromObjectAsJson(data);
        }
    }
}
