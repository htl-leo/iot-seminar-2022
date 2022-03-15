using Core.Entities;

using IotServices.DataTransferObjects;

using System.Threading.Tasks;

using Wasm.Services.Contracts;

namespace Wasm.DataTransferObjects
{
    /// <summary>
    /// Dto als Basis für das DataBinding von Aktoren
    /// Nachdem der Sollzustand des Aktors gesetzt wird,
    /// wird sein UI disabled, bis der Aktor die gewünschte
    /// Änderung zurückmeldet.
    /// </summary>
    public class ActorUiDto
    {
        public IApiService ApiService { get; set; } // wird per ctor gesetzt
        public ItemEnum ActorEnum { get; set; }
        public string Name => ActorEnum.ToString();
        public bool IsDisabled => WaitingForResponse;
        public bool WaitingForResponse { get; set; }
        public bool IsOn { get; set; }

        public ActorUiDto(ItemEnum actorEnum, IApiService apiService)
        {
            ApiService = apiService;
            ActorEnum = actorEnum;
        }
        public void NewActorValueReceived(MeasurementTimeValue measurement)
        {
            WaitingForResponse = false;
            IsOn = measurement.Value == 1;
            System.Console.WriteLine($"{Name}, NewActorValueReceived, IsOn: {IsOn}, WaitingForResponse: {WaitingForResponse}");
        }
        public async Task SwitchActorPerApiAsync()
        {
            await ApiService.ChangeSwitchAsync(Name, IsOn);
            WaitingForResponse = true;
            System.Console.WriteLine($"{Name}, SwitchActorPerApi, IsOn: {IsOn}, WaitingForResponse: {WaitingForResponse}");
        }
    }
}
