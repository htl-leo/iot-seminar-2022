using Base.ExtensionMethods;

using Core.Entities;

using IotServices.Services;

using Serilog;

using System;

namespace IotServices.DataTransferObjects
{
    public class MeasurementDto
    {
        public Item? Item { get; set; }
        public ItemEnum ItemEnum { get; set; }
        public string ItemName => ItemEnum.ToString();

        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value.ToLegalDouble();
            }
        }

        public DateTime Time { get; set; }

        public MeasurementDto()
        {

        }
        public MeasurementDto(Measurement measurement, StateService stateService)
        {
            Value = measurement.Value;
            Time = measurement.Time;
            ItemEnum = Enum.Parse<ItemEnum>(measurement.Item.Name);
            var item = stateService.GetSensor(measurement.Item.ItemEnum);
            if (item != null)
            {
                Item = item;    
            }
            else
            {
                Log.Error($"Create MeasurementDto; Item {measurement.Item.Name} doesn't exits");
            }
        }


        public override string ToString()
        {
            return $"{ItemName} {Time.ToShortTimeString()}: {Value}";
        }
    }
}
