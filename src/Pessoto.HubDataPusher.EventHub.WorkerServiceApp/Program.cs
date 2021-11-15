using Pessoto.HubDataPusher.Core;
using Pessoto.HubDataPusher.EventHub.Core;
using Pessoto.HubDataPusher.EventHub.WorkerServiceApp;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        ConfigureOptions(hostContext, services);

        AddHubDataGenerator(services, hostContext.Configuration["HubDataGenerator:Type"]);

        services.AddTransient<BandwitdhThrottler, BandwitdhThrottler>();
        services.AddTransient<EventHubDataPusher, EventHubDataPusher>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

static void ConfigureOptions(HostBuilderContext hostContext, IServiceCollection services)
{
    services.Configure<EventHubDataPusherOptions>(hostContext.Configuration.GetSection("EventHubDataPusher"));
    services.Configure<BandwitdhThrottlerOptions>(hostContext.Configuration.GetSection("BandwitdhThrottler"));
    services.Configure<DynamicSchemaHubDataGeneratorOptions>(hostContext.Configuration.GetSection("HubDataGenerator:DynamicSchemaHubDataGenerator"));
    services.Configure<BigEventsHubDataGeneratorOptions>(hostContext.Configuration.GetSection("HubDataGenerator:BigEventsHubDataGenerator"));
    services.Configure<StaticDataHubDataGeneratorOptions>(hostContext.Configuration.GetSection("HubDataGenerator:StaticDataHubDataGenerator"));
}

static void AddHubDataGenerator(IServiceCollection services, string dataGeneratorType)
{
    if (dataGeneratorType == "StaticDataHubDataGenerator")
    {
        services.AddTransient<IHubDataGenerator, StaticDataHubDataGenerator>();
    }
    else if (dataGeneratorType == "SmallEventsHubDataGenerator")
    {
        services.AddTransient<IHubDataGenerator, SmallEventsHubDataGenerator>();
    }
    else if (dataGeneratorType == "BigEventsHubDataGenerator")
    {
        services.AddTransient<IHubDataGenerator, BigEventsHubDataGenerator>();
    }
    else if (dataGeneratorType == "DynamicSchemaHubDataGenerator")
    {
        services.AddTransient<IHubDataGenerator, DynamicSchemaHubDataGenerator>();
    }
    else
    {
        throw new InvalidOperationException($"Invalid HubDataGenerator.Type: {dataGeneratorType}");
    }
}
