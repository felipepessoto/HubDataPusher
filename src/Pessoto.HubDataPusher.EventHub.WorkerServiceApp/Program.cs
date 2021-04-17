using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pessoto.HubDataPusher.Core;
using Pessoto.HubDataPusher.EventHub.Core;
using System;

namespace Pessoto.HubDataPusher.EventHub.WorkerServiceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<EventHubDataPusherOptions>(hostContext.Configuration.GetSection("EventHubDataPusher"));

                    string dataGeneratorType = hostContext.Configuration["HubDataGenerator:Type"];
                    if (dataGeneratorType == "SampleHubDataGenerator")
                    {
                        services.AddTransient<IHubDataGenerator, SampleHubDataGenerator>();
                    }
                    else if (dataGeneratorType == "StaticDataHubDataGenerator")
                    {
                        services.AddTransient<IHubDataGenerator, StaticDataHubDataGenerator>();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid HubDataGenerator.Type: {dataGeneratorType}");
                    }

                    services.AddTransient<EventHubDataPusher, EventHubDataPusher>();
                    services.AddHostedService<Worker>();
                });
    }
}
