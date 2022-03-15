using Core.Entities;
using IotServices.DataTransferObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Wasm.DataTransferObjects;

namespace Wasm.Pages.IoT
{
    public partial class Dashboard
    {
        public ActorUiDto IotShellyPlug_Relay { get; set; }

        private HubConnection hubConnection;

        [Inject]
        public IConfiguration Configuration { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IotShellyPlug_Relay = new ActorUiDto(ItemEnum.IotShellyPlug_Relay);

            Console.WriteLine($"------------------- SignalRHubUrl: {Configuration["SignalRHubUrl"]}");
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Configuration["SignalRHubUrl"])
                .Build();

            hubConnection.On<MeasurementTimeValue>("ReceiveMeasurement", (measurement) =>
            {
                if (measurement.ItemName == IotShellyPlug_Relay.ActorEnum.ToString())
                {
                    IotShellyPlug_Relay.IsOn = measurement.Value != 0;
                    Console.WriteLine($"Switch changed to {IotShellyPlug_Relay.IsOn}");
                    StateHasChanged();
                }
                Console.WriteLine($"{measurement.ItemName} {measurement.Value}");

            });
            await hubConnection.StartAsync();
        }
        protected async Task SwitchChangedAsync(ItemEnum actorEnum)
        {
            // derzeit nur ein Actor
            Console.WriteLine($"{actorEnum} switch changed");
            // temp code: switch IsOn-state manually
            IotShellyPlug_Relay.IsOn = !IotShellyPlug_Relay.IsOn;
            Console.WriteLine($"{actorEnum} switch changed: {IotShellyPlug_Relay.IsOn}");
            StateHasChanged();
        }
    }
}
