
using System;
using System.Threading.Tasks;

namespace IotServices.Contracts
{
    public interface IMqttService
    {
        Task InitAsync(string serverUrl, int port, string mqttUser, string mqttPassword,
            Action<string, string, int, bool> receivedMqttMessageHandler);
    }
}