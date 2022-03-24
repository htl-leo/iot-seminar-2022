using Core.Contracts;
using Core.Entities;

using IotServices.Contracts;
using IotServices.DataTransferObjects;
using IotServices.Services.MqttAdapter;

using Persistence;

using Serilog;

using Services.Contracts;

namespace IotServices.Services
{

    public class StateService : IStateService
    {

        public StateService(MqttService mqttService)
        {
            MqttService = mqttService;
        }

        private MqttService MqttService { get; }

        public Dictionary<ItemEnum, MeasurementDto?> LastMeasurements { get; private set; }

        public Sensor[] Sensors { get; private set; }
        public Actor[] Actors { get; private set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public Sensor? GetSensor(ItemEnum sensorEnum) => Sensors.SingleOrDefault(s => s.ItemEnum == sensorEnum);
        public Actor? GetActor(ItemEnum actorEnum) => Actors.SingleOrDefault(a => a.ItemEnum == actorEnum);

        public Item? GetItem(ItemEnum itemEnum)
        {
            var item = GetSensor(itemEnum) as Item ?? GetActor(itemEnum);
            return item;
        }

        public MeasurementDto? GetLastMeasurement(ItemEnum itemEnum) => LastMeasurements[itemEnum];

        public async Task InitAsync()
        {
            Log.Information("StateService;Init;StateService started");
            var itemEnums = Enum.GetValues<ItemEnum>();
            Sensors = itemEnums
                .Where(ie => ie < ItemEnum.EndOfSensor)
                .Select(ie => new Sensor(ie, ""))
                .ToArray();
            Actors = itemEnums
                .Where(ie => ie > ItemEnum.EndOfSensor)
                .Select(ie => new Actor(ie, ""))
                .ToArray();
            foreach (var sensor in Sensors)
            {
                await UnitOfWork.SensorRepository.SynchronizeAsync(sensor);
            }
            foreach (var actor in Actors)
            {
                await UnitOfWork.ActorRepository.SynchronizeAsync(actor);
            }
            LastMeasurements = new();
            foreach (var sensor in Sensors)
            {
                LastMeasurements[sensor.ItemEnum] = null;
            }
            foreach (var actor in Actors)
            {
                LastMeasurements[actor.ItemEnum] = null;
            }
            MqttService.MessageReceived += Mqtt_MessageReceived;
        }

        private async void Mqtt_MessageReceived(object? sender, MqttMessage mqttMessage)
        {
            MeasurementDto measurementDto;
            if (mqttMessage.Topic.ToLower().Contains("shell"))
            {
                measurementDto = ShellyMqttAdapter.ConvertMqttToMeasurementDto(
                        mqttMessage.Topic, mqttMessage.Payload);
                // erzeuge measurement aus shelly-daten
            }
            else
            {
                measurementDto = EspSensorBoxMqttAdapter.ConvertMqttToMeasurementDto(
                        mqttMessage.Topic, mqttMessage.Payload);
            }
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
                await UnitOfWork.MeasurementRepository.AddAsync(dbMeasurement);
                await UnitOfWork.SaveChangesAsync();
            }
            else
            {
                Log.Error($"Mqtt_MessageReceived, Item: {itemEnum} not found");
            }
        }

    }

}
