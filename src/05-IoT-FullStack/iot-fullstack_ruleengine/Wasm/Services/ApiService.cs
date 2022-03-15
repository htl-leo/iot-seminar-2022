
using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Wasm.Services.Contracts;

namespace Wasm.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;

        public ApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> ChangeSwitchAsync(string name, bool on)
        {
            var onOffValue = 0;
            if (on)
            {
                onOffValue = 1;
            }
            var request = $"api/actors/change/{name},{onOffValue}";
            var response = await _client.GetAsync(request);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(contentTemp);
            Console.WriteLine($"ApiService;ChangeSwitchAsync; Request: {request}; result: {result}");
            return result;
        }

        public async Task<IotServices.DataTransferObjects.MeasurementTimeValue[]> GetMeasurementsAsync(string sensorName, DateTime date)
        {
            var request = $"api/measurements/getbysensoranddate/{sensorName},{date:yyyy-MM-dd}";
            var response = await _client.GetAsync(request);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IotServices.DataTransferObjects.MeasurementTimeValue[]>(contentTemp);
            System.Console.WriteLine($"ApiService;GetMeasurementsAsync; {result.Length} measurements read");
            return result;
        }

    }

}
