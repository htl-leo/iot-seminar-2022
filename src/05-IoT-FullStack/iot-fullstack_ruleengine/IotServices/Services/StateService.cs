using Core.Entities;

using IotServices.Contracts;
using IotServices.DataTransferObjects;
using IotServices.Services.MqttAdapter;

using Microsoft.AspNetCore.SignalR;

using Serilog;

using Services.Contracts;
using Services.Hubs;

namespace IotServices.Services
{

    public class StateService : IStateService
    {
        private static readonly Lazy<StateService> lazy = new(() => new StateService());
        public static StateService Instance { get { return lazy.Value; } }
        IHubContext<SignalRHub>? SignalRHubContext { get; set; } // wird vom IotService per Property injiziert

        public EspMqttAdapter EspMqttAdapter { get; }
        public ShellyMqttAdapter ShellyMqttAdapter { get; }


        private StateService()
        {
            EspMqttAdapter = new();
            ShellyMqttAdapter = new();
            var itemEnums = Enum.GetValues<ItemEnum>();
            Sensors = itemEnums
                .Where(ie => ie < ItemEnum.EndOfSensor)
                .Select(ie => new Sensor(ie, ""))
                .ToArray();
            Actors = itemEnums
                .Where(ie => ie > ItemEnum.EndOfSensor)
                .Select(ie => new Actor(ie, ""))
                .ToArray();
            LastMeasurements = new();
            foreach (var sensor in Sensors)
            {
                LastMeasurements[sensor.ItemEnum] = null;
            }
            foreach (var actor in Actors)
            {
                LastMeasurements[actor.ItemEnum] = null;
            }
        }

        public Dictionary<ItemEnum, MeasurementTimeValue?> LastMeasurements { get; }

        public Sensor[] Sensors { get; }
        public Actor[] Actors { get; }

        public Sensor? GetSensor(ItemEnum sensorEnum) => Sensors.SingleOrDefault(s => s.ItemEnum == sensorEnum);
        public Actor? GetActor(ItemEnum actorEnum) => Actors.SingleOrDefault(a => a.ItemEnum == actorEnum);

        public Item? GetItem(ItemEnum itemEnum)
        {
            var item = GetSensor(itemEnum) as Item ?? GetActor(itemEnum);
            return item;
        }

        public MeasurementTimeValue? GetLastMeasurement(ItemEnum itemEnum) => LastMeasurements[itemEnum];

        public void Init(IHubContext<SignalRHub> signalRHubContext)
        {
            Log.Information("StateService;Init;StateService started");
            // Sensoren und Aktoren mit DB synchronisieren
            foreach (var sensor in Sensors)
            {
                PersistenceService.Instance.UnitOfWork.SensorRepository.SynchronizeAsync(sensor);
            }
            foreach (var actor in Actors)
            {
                PersistenceService.Instance.UnitOfWork.ActorRepository.SynchronizeAsync(actor);
            }
            // SignalR-Hub initialisieren
            SignalRHubContext = signalRHubContext;
            MqttService.Instance.MessageReceived += Mqtt_MessageReceived;
        }

        private async void Mqtt_MessageReceived(object? sender, MqttMessage mqttMessage)
        {
            MeasurementTimeValue? measurementDto;
            if (mqttMessage.Topic.ToLower().Contains("shellies"))
            {

                measurementDto = ShellyMqttAdapter.MqttToMeasurementDto(
                        mqttMessage.Topic, mqttMessage.Payload);
            }
            else
            {
                measurementDto = EspMqttAdapter.MqttToMeasurementDto(
                        mqttMessage.Topic, mqttMessage.Payload);
            }
            if (measurementDto != null)  // Mqtt-Sensoren, die nicht gebraucht werden ignorieren
            {
                LastMeasurements[measurementDto.ItemEnum] = measurementDto;
                ItemEnum itemEnum = measurementDto.ItemEnum;
                Item? item = GetItem(itemEnum);
                if (item != null)
                {
                    var dbMeasurement = new Measurement
                    {
                        ItemId = item.Id,
                        Time = measurementDto.Time,
                        Value = measurementDto.Value,
                    };
                    await PersistenceService.Instance.UnitOfWork.MeasurementRepository.AddAsync(dbMeasurement);
                    await PersistenceService.Instance.UnitOfWork.SaveChangesAsync();
                    if (SignalRHubContext != null)
                    {
                        await SignalRHubContext.Clients.All.SendAsync("ReceiveMeasurement", measurementDto);
                    }
                    Log.Information($"Mqtt_MessageReceived, Item: {itemEnum}, Value: {dbMeasurement.Value} saved");
                }
                else
                {
                    Log.Error($"Mqtt_MessageReceived, Item: {itemEnum} not found");
                }
            }
        }

        /// <summary>
        /// Setzt den Aktor per Mqtt auf den gewünschten Zustand
        /// </summary>
        /// <param name="actorName"></param>
        /// <param name="value"></param>
        /// <returns>false, wenn der Aktor nicht existiert</returns>
        public async Task<bool> SetActorAsync(string actorName, double value)
        {
            if (!Enum.TryParse<ItemEnum>(actorName, out ItemEnum itemEnum))
            {
                return false;
            }
            Actor? actor = GetActor(itemEnum);
            if (actor == null)
            {
                return false;
            }
            // Mqtt-Message über Adapter erzeugen lassen (Shelly-Adapter)
            (string topic, string payload) = ShellyMqttAdapter.GenerateMqttMessage(itemEnum, value);
            //! Actor per Mqtt setzen
            await MqttService.Instance.PublishAsync(topic, payload);
            Log.Information($"SetActorAsync; {actorName}:{value}");
            return true;
        }



    }

}
