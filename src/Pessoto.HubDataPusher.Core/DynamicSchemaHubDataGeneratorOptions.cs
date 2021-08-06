namespace Pessoto.HubDataPusher.Core
{
    public class DynamicSchemaHubDataGeneratorOptions
    {
        public string IdPropertyName { get; set; } = "";

        public string IdPropertyValuePrefix { get; set; } = "";

        public int NumberOfIds { get; set; }

        public string ValuePropertyName { get; set; } = "";

        public int NumberOfProperties { get; set; }
    }
}
