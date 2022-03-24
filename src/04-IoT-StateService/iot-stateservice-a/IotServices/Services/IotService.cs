using Base.Helper;
using Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace IotServices.Services
{
    public class IotService : BackgroundService
    {
        public IotService(IServiceScopeFactory  factory)
        {
            Factory = factory;
        }

        public IServiceScopeFactory Factory { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("IotService;ExecuteAsync;started");

            using var childContainer = Factory.CreateScope();
            var uow = childContainer.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var stateService = childContainer.ServiceProvider.GetRequiredService<StateService>();
            var mqttService = childContainer.ServiceProvider.GetRequiredService<MqttService>();
            var persistenceService = childContainer.ServiceProvider.GetRequiredService<PersistenceService>();
            stateService.UnitOfWork = uow;
            await mqttService!.InitAsync();
            await stateService!.InitAsync(); // Sensoren/Aktoren initialisieren und mit DB synchronisieren


            int round = 0;
            while (!stoppingToken.IsCancellationRequested)  // Pollingloop every 100ms
            {
                await Task.Delay(100, stoppingToken);
                round++;
                if (round >= 100)  // alle 10 Sekunden
                {
                    Log.Information($"IotService;ExecuteAsync; 10 seconds have passed");
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
