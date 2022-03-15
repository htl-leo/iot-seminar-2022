using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Core.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using IotServices.DataTransferObjects;
using Wasm.DataTransferObjects;
using Wasm.Services.Contracts;

namespace Wasm.Pages.IoT
{
    public partial class Dashboard : IAsyncDisposable
    {
        [Inject]
        public IApiService ApiService { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        private HubConnection hubConnection;

        public MeasurementTimeValue Temperature { get; set; } = new MeasurementTimeValue();
        public MeasurementTimeValue Rssi { get; set; } = new MeasurementTimeValue();

        public MeasurementTimeValue IotShellyPlug_Power { get; set; } = new MeasurementTimeValue();
        public ActorUiDto IotShellyPlug_Relay { get; set; }


        protected override async Task OnInitializedAsync()
        {
            IotShellyPlug_Relay = new ActorUiDto(ItemEnum.IotShellyPlug_Relay, ApiService);

            Console.WriteLine($"------------------- SignalRHubUrl: {Configuration["SignalRHubUrl"]}");
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Configuration["SignalRHubUrl"])
                .Build();

            hubConnection.On<MeasurementTimeValue>("ReceiveMeasurement", (measurement) =>
            {
                var encodedMsg = $"{measurement.ItemName}, {(int)measurement.ItemEnum}, {measurement.Time}, {measurement.Value}";
                Console.WriteLine(encodedMsg);
                //var itemEnum = Enum.Parse<ItemEnum>(measurement.ItemName, false);
                switch (measurement.ItemEnum)
                {
                    case ItemEnum.Seminar_Temperature:
                        Temperature.Value = measurement.Value;
                        break;
                    case ItemEnum.Seminar_Rssi:
                        Rssi.Value = measurement.Value;
                        //Console.WriteLine("Rssi received");
                        break;
                    case ItemEnum.IotShellyPlug_Relay:
                        IotShellyPlug_Relay.NewActorValueReceived(measurement);
                        break;
                    case ItemEnum.IotShellyPlug_Power:
                        IotShellyPlug_Power.Value = measurement.Value;
                        break;
                    default:
                        break;
                }
                StateHasChanged();
            });
            await hubConnection.StartAsync();
        }

        protected async Task SwitchChangedAsync(ItemEnum actorEnum)
        {
            // derzeit nur ein Actor
            await IotShellyPlug_Relay.SwitchActorPerApiAsync();
        }

       public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                Console.WriteLine("SignalR-Connection disposed!");
                await hubConnection.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

    }
}

