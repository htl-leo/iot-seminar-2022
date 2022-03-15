using Base.ExtensionMethods;
using Core.Entities;
using IotServices.DataTransferObjects;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotServices.Services.MqttAdapter
{
    public class ShellyMqttAdapter
    {
        public static MeasurementDto ConvertMqttToMeasurementDto(string topic, string payload)
        {
            // shellies/iotshellyplug/relay/0/power => IotShellyPlug_Power,
            // ... energy
            // payload: 0.00
            try
            {
                var last = topic.Substring(topic.LastIndexOf('/') + 1);
                var itemName = "IotShellyPlug_" + last;
                var enumValue = Enum.Parse<ItemEnum>(itemName, true);
                var value = payload.TryParseToDouble();
                if (value == null)
                {
                    Log.Error($"ConvertMqttToMeasurementDto, payload value not parseable to double {payload}");
                    return new();
                }
                else
                {
                    return new MeasurementDto
                    {
                        ItemEnum = enumValue,
                        Value = value.Value,
                        Time = DateTime.Now
                    };
                }
            }
            catch (Exception ex)
            {
                return new();
            }

        }
    }
}
