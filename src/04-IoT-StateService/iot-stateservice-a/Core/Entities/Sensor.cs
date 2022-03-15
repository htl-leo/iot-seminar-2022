namespace Core.Entities
{
    //public enum SensorName
    //{
    //    Office_Humidity = 1,
    //    Office_Temperature,
    //    Office_Motion,
    //    Office_Pressure,
    //    Office_Co2,
    //    Office_Rssi,
    //    IotShellyPlug_Power,
    //}


    /// <summary>
    /// Sensoren werden über die Enum SensorName typsicher benannt.
    /// </summary>
    public class Sensor : Item
    {
        public Sensor(ItemEnum itemEnum, string unit) : base(itemEnum, unit)
        {
        }

        public Sensor()  { }

        //public Sensor(SensorName sensorName, string unit = "") 
        //        : base((int) sensorName, unit)
        //{
        //    Name = sensorName.ToString();
        //}


        public override string ToString() => $"Sensor: {Name}";

    }
}
