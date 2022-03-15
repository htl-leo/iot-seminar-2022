using System.Text;

using IotServices.Contracts;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;

using Serilog;

namespace IotServices.Services
{
    public class MqttService : IMqttService
    {
        public IMqttClientOptions? MqttClientOptions { get; private set; }
        private readonly string ClientId = $"MqttService_{Guid.NewGuid()}";


        /// <summary>
        /// Mit Mqttbroker verbinden und einen Subscriber anmelden.
        /// Rückgabe der Messages über Callback
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="port"></param>
        /// <param name="mqttUser"></param>
        /// <param name="mqttPassword"></param>
        /// <param name="receivedMqttMessageHandler"></param>
        public async Task InitAsync(string serverUrl, int port, string mqttUser, string mqttPassword,
            Action<string, string, int, bool> receivedMqttMessageHandler)
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            #region Create TCP based options using the builder
            MqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithTcpServer(serverUrl, port)
                .WithCredentials(mqttUser, mqttPassword)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    UseTls = true,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12  //  neu mit MQTTnet 3.0.15
                })
                .WithCleanSession()
                //.WithCommunicationTimeout(TimeSpan.FromSeconds(2))
                .Build();
            #endregion

            mqttClient.UseConnectedHandler(async e =>
            {
                Log.Information("Mqtt connected");
                // Subscribe to all topics
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#").Build());
                Log.Information("all topics subscribed");
            });
            mqttClient.UseApplicationMessageReceivedHandler(mqttEvent =>
            {
                string topic = mqttEvent.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(mqttEvent.ApplicationMessage.Payload);
                var qos = mqttEvent.ApplicationMessage.QualityOfServiceLevel;
                var retained = mqttEvent.ApplicationMessage.Retain;
                if (retained)
                {
                    return;
                }
                Log.Information("Mqtt message received, topic: {topic}, payload: {payload}",
                    topic, payload);
                receivedMqttMessageHandler(topic, payload, (int)qos, retained);
            });
            mqttClient.UseDisconnectedHandler(async e =>
            {
                Log.Error("Mqtt disconnected, {Reasoncode}", e.Reason);
                // Reconnect
                await mqttClient.ConnectAsync(MqttClientOptions, CancellationToken.None); // Since 3.0.5 with CancellationToken
            });
            try  // Connect with broker
            {
               var result = await mqttClient.ConnectAsync(MqttClientOptions, CancellationToken.None); // Since 3.0.5 with CancellationToken
                if (result.ResultCode != MqttClientConnectResultCode.Success)
                {
                    Log.Error("MqttService, Init, ConnectAsync, ErrorCode: {MqttResultcode}", result.ResultCode);
                }
            }
            catch (Exception ex)
            {
                Log.Error("MqttService, Init, Exception: {Exception}", ex.Message);
                StringBuilder innerExceptionMessages = new();
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    innerExceptionMessages.Append(innerEx.Message);
                    innerExceptionMessages.Append('*');
                    innerEx = innerEx.InnerException;
                }
                Log.Error("MqttService, Init, InnerExceptions: {InnerException}", innerExceptionMessages.ToString());
            }
            Log.Information("MqttService; Init(); MqttClientId: {MqttClientId} ", ClientId);
        }

    }
}
