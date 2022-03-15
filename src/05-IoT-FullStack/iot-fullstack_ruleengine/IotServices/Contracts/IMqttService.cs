
using System;
using System.Threading.Tasks;

namespace IotServices.Contracts
{
    public record MqttMessage(string Topic, string Payload, int Qos, bool Retained);

    public interface IMqttService
    {
        Task InitAsync();
        Task<bool> PublishAsync(string topic, string value);

        public event EventHandler<MqttMessage>? MessageReceived;

    }
}