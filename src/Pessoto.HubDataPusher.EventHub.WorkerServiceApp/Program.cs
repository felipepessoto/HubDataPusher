using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pessoto.HubDataPusher.Core;
using Pessoto.HubDataPusher.EventHub.Core;

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
                    services.AddTransient<IHubDataGenerator, SampleHubDataGenerator>();
                    //services.AddTransient<IHubDataGenerator, StaticDataHubDataGenerator>();
                    services.AddTransient<EventHubDataPusher, EventHubDataPusher>();
                    services.AddHostedService<Worker>();
                });
    }
}
