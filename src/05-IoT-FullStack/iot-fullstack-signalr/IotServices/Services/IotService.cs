using Base.Helper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

using Serilog;
using Services.Hubs;

namespace IotServices.Services
{
    public class IotService : BackgroundService
    {
        readonly IHubContext<SignalRHub> _signalRHubContext;

        public IotService(IHubContext<SignalRHub> hubContext)
        {
            _signalRHubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("IotService;ExecuteAsync;started");

            _ = PersistenceService.Instance;
            await MqttService.Instance.InitAsync();
            StateService.Instance.Init(); // Sensoren/Aktoren initialisieren und mit DB synchronisieren
            StateService.Instance.SignalRHubContext = _signalRHubContext;

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