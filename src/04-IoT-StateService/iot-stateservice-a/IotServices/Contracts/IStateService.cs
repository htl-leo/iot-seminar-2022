using Core.Entities;

namespace Services.Contracts
{
    public interface IStateService
    {
        //public event EventHandler<Measurement> NewMeasurement;

        Sensor[] Sensors { get; }
        Actor[] Actors { get; }

        Sensor? GetSensor(ItemEnum sensorName);
        Actor? GetActor(ItemEnum actorName);

        //void Init(ISerialCommunicationService serialCommunicationService, IEspHttpCommunicationService espHttpCommunicationService,
        //    IHomematicHttpCommunicationService homematicHttpCommunicationService);

        Task InitAsync();

        //Task SendItemsBySignalRAsync();

        //Measurement[] GetMeasurementsToSave(Item[] items);
        //Measurement[] GetActorMeasurementsToSave();
        //MeasurementDto[] GetSensorAndActorValues();
    }
}
