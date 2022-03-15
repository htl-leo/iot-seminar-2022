using Core.Contracts;

using Persistence;

namespace IotServices.Services
{
    public class PersistenceService
    {
        private static readonly Lazy<PersistenceService> lazy = new(() => new PersistenceService());
        public static PersistenceService Instance { get { return lazy.Value; } }
        private PersistenceService() 
        {
            UnitOfWork = new UnitOfWork();
            UnitOfWork.SetNoTracking();
        }

        public IUnitOfWork UnitOfWork { get; set; }

        //private async Task<Sensor[]> SyncSensors(List<Sensor> initialSensors)
        //{
        //    try
        //    {
        //        var dbSensors = (await UnitOfWork.SensorRepository.GetAsync()).ToList();
        //        Log.Information($"RuleEngine;SyncSensors;{dbSensors.Count} sensors read from db");
        //        var sensorsToDelete = dbSensors
        //            .Where(s => !initialSensors.Any(initialSensor => initialSensor.Name == s.Name))
        //            .ToArray();
        //        foreach (var item in sensorsToDelete)
        //        {
        //            UnitOfWork.SensorRepository.Remove(item);
        //        }
        //        var sensorsToInsert = initialSensors
        //            .Where(s => !dbSensors.Any(dbSensor => dbSensor.Name == s.Name))
        //            .ToArray();
        //        await UnitOfWork.SensorRepository.AddRangeAsync(sensorsToInsert);
        //        await UnitOfWork.SaveChangesAsync();
        //        var sensors = await UnitOfWork.SensorRepository.GetAsync();
        //        var sensorNames = Enum.GetNames(typeof(SensorName));
        //        if (sensors.Length != sensorNames.Length)
        //        {
        //            Log.Error($"RuleEngine,SyncSensors; dbSensors: {sensors.Length} !=  enumSensors: {sensorNames.Length}");
        //            return null;
        //        }
        //        var sensorArray = new Sensor[sensorNames.Length];
        //        for (int i = 0; i < sensorNames.Length; i++)
        //        {
        //            sensorArray[i] = sensors.Single(s => s.Name == ((SensorName)i).ToString());
        //        }
        //        return sensorArray;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"RuleEngine,SyncSensors;Failed to read sensors; ex: {ex.Message}");
        //    }
        //    return null;
        //}

    }
}
