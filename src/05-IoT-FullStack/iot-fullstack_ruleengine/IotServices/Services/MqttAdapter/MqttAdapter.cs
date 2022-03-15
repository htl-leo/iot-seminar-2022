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
    public abstract class MqttAdapter
    {
        record MqttPayload(long Timestamp, double Value);

        public abstract MeasurementTimeValue? MqttToMeasurementDto(string topic, string jsonPayload);

        public abstract (string topic, string payload) GenerateMqttMessage(ItemEnum item, double value);

    }
}
