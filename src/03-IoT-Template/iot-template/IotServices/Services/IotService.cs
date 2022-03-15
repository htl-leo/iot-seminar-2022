using Base.Helper;

using Microsoft.Extensions.Hosting;

using Serilog;

namespace IotServices.Services
{
    public class IotService : BackgroundService
    {
        public IServiceProvider ServiceProvider { get; }  // DI-Provider

        public IotService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("IotService;ExecuteAsync;started");
            // connect mqtt 
            var mqttService = new MqttService();
            string broker = ConfigurationHelper.GetConfiguration( "Broker", "Mqtt");
            int port = int.Parse(ConfigurationHelper.GetConfiguration ("Port", "Mqtt"));
            string user = ConfigurationHelper.GetConfiguration( "User", "Mqtt");
            string password = ConfigurationHelper.GetConfiguration( "Password", "Mqtt");
            await mqttService.InitAsync(broker, port, user, password,
                ReceiveMqttMessage);

            int round = 0;
            while (!stoppingToken.IsCancellationRequested)  // Pollingloop every 100ms
            {
                await Task.Delay(100, stoppingToken);
                round++;
                if (round >= 100)  // alle 10 Sekunden
                {
                    Log.Information($"RuleEngine;ExecuteAsync; 10 seconds have passed");
                    round = 0;
                }
            }
        }

        public static void ReceiveMqttMessage(string topic, string payload, int qos, bool retained)
        {
            //return;
            Log.Information("ReceiveMqttMessage, topic: {Topic}, payload: {payload}",
                topic, payload);
        }

    }
}