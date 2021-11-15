# HubDataPusher

## How to use 
1. Edit appsettings.json

Required:

- **EventHubDataPusher.ConnectionString** - Event Hub connection string. Example: Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=AccessKeyName;SharedAccessKey=Abc=;EntityPath=hub-name

Optional:

- **HubDataGenerator.Type** - It currently provides two types: SmallEventsHubDataGenerator, StaticDataHubDataGenerator, BigEventsHubDataGenerator and DynamicSchemaHubDataGenerator
- **EventHubDataPusher.MaximumBatchSize** - Maximum batch size. Can't be bigger than specified in https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-quotas
- **EventHubDataPusher.NumberOfThread** - The number of threads pushing data
- **BandwitdhThrottler.Enabled** - Enable bandwitdh throttler
- **BandwitdhThrottler.MaxPushRateMBps** - If enabled, limits the push rate by the value specified (MB per second)

2. Start Pessoto.HubDataPusher.EventHub.WorkerServiceApp.exe
3. Monitor the push rate and number of events sent using Event Counters (the built-in .NET Core metric collection framework). The easiest way to read Event Counters is using [dotnet-counters](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters):

Install:

``dotnet tool install --global dotnet-counters``

After starting the pusher, run:

``dotnet-counters.exe monitor --refresh-interval 1 -n Pessoto.HubDataPusher.EventHub.WorkerServiceApp --counters Pessoto.HubDataPusher.Core.DataPusher System.Runtime``

![image](https://user-images.githubusercontent.com/1336227/115163524-f1072480-a07f-11eb-9972-f03b67a3da84.png)
