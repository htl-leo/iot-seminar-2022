using Base.Helper;

using Core.Entities;

using IotServices.DataTransferObjects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Base.ExtensionMethods;

namespace IotServices.Services.MqttAdapter
{
    public class ShellyMqttAdapter : MqttAdapter
    {

        public override MeasurementTimeValue? MqttToMeasurementDto(string topic, string jsonPayload)
        {

            // shellies/iotshellyplug/relay/0/power
            // shellies/iotshellyplug/relay/0
            // 33.22

            var dto = new MeasurementTimeValue();
            if (topic.Contains("shellies/iotshellyplug/relay/0/power"))
            {
                double? value = jsonPayload.TryParseToDouble();
                if (value == null)
                {
                    return null;
                }
                dto.ItemEnum = ItemEnum.IotShellyPlug_Power;
                dto.Value = value.Value;
                dto.Time = DateTime.Now;
            }
            else if (topic == "shellies/iotshellyplug/relay/0")
            {
                dto.ItemEnum = ItemEnum.IotShellyPlug_Relay;
                dto.Value = 0.0;
                if (jsonPayload.ToLower() == "on")
                {
                    dto.Value = 1.0;
                }
                dto.Time = DateTime.Now;
            }
            else
            {
                return null;
            }
            return dto;
        }

        public override (string topic, string payload) GenerateMqttMessage(ItemEnum item, double value)
        {
            if (item == ItemEnum.IotShellyPlug_Relay)
            {
                string onOff = "off";
                if (value > 0)
                {
                    onOff = "on";
                }
                return ("shellies/iotshellyplug/relay/0/command", onOff);
            }
            return ("", "");
        }
    }
}
