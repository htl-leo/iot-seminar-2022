using Base.Helper;

using Microsoft.Extensions.Hosting;

using Serilog;

namespace IotServices.Services
{
    public class IotService : BackgroundService
    {
        public IotService() {}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("IotService;ExecuteAsync;started");

            _ = PersistenceService.Instance;
            await MqttService.Instance.InitAsync();
            StateService.Instance.Init(); // Sensoren/Aktoren initialisieren und mit DB synchronisieren


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

        //public static void ReceiveMqttMessage(string topic, string payload, int qos, bool retained)
        //{
        //    //return;
        //    Log.Information("ReceiveMqttMessage, topic: {Topic}, payload: {payload}",
        //        topic, payload);
        //}

    }
}