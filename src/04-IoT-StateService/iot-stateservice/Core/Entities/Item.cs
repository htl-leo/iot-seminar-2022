using Base.Entities;


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public record MeasurementValue(DateTime Time, double Value);

    public enum ItemEnum
    {
        Seminar_Humidity = 1,
        Seminar_Temperature,
        Seminar_Motion,
        Seminar_Pressure,
        Seminar_Co2,
        Seminar_Rssi,
        Seminar_Luminosity,
        IotShellyPlug_Power,
        
        EndOfSensor = 100,

        IotShellyPlug_Relay = 101,

    }

    /// <summary>
    /// Item ist die Basisiklasse für Sensor und Actor 
    /// </summary>
    public abstract class Item : EntityObject
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        [NotMapped]
        public ItemEnum ItemEnum { get; private set; }

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

        public Item()
        {
        }

        public Item(ItemEnum itemEnum, string unit = "")
        {
            ItemEnum = itemEnum;
            Name = itemEnum.ToString();
            Unit = unit;
        }
    }
}
