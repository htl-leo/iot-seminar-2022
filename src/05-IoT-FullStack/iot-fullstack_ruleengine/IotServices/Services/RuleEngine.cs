using Core.Entities;

using Microsoft.Extensions.Hosting;

using Serilog;

using Services.Contracts;
using Services.Fsm;

namespace Services
{
    public class RuleEngine
    {
        const int BAD_AIR_QUALITY_CO2 = 700;
        const int DANGEROUS_AIR_QUALITY_CO2 = 1700;
        const int HYSTERESE = 100;

        private static readonly Lazy<RuleEngine> lazy = new(() => new RuleEngine());
        public static RuleEngine Instance { get { return lazy.Value; } }
        private RuleEngine()
        {
            StateService = IotServices.Services.StateService.Instance;
            Enum[] stateEnums = Enum.GetValues<State>().Select(e => (Enum)e).ToArray();
            Enum[] inputEnums = Enum.GetValues<Input>().Select(e => (Enum)e).ToArray();
            IndoorClimateFsm = new FiniteStateMachine(nameof(IndoorClimateFsm), stateEnums, inputEnums);
        }
        public IStateService StateService { get; }



        public FiniteStateMachine IndoorClimateFsm { get; }

        public enum State { GoodAirQuality, BadAirQuality, DangerousAirQuality };
        public enum Input
        {
            AirQualityIsGettingBad, AirQualityIsGettingDangerous, AirQualityIsGettingBetter, AirQualityIsGettingGood
        };

        public void InitIndoorClimateFsm()
        {
            try
            {
                // Triggermethoden bei Inputs definieren
                var inputAirQualityIsGettingBad = IndoorClimateFsm.GetInput(Input.AirQualityIsGettingBad);
                inputAirQualityIsGettingBad.TriggerMethod = AirQualityIsGettingBad;
                var inputAirQualityIsGettingDangerous = IndoorClimateFsm.GetInput(Input.AirQualityIsGettingDangerous);
                inputAirQualityIsGettingDangerous.TriggerMethod = AirQualityIsGettingDangerous;
                var inputAirQualityIsGettingBetter = IndoorClimateFsm.GetInput(Input.AirQualityIsGettingBetter);
                inputAirQualityIsGettingBetter.TriggerMethod = AirQualityIsGettingBetter;
                var inputAirQualityIsGettingGood = IndoorClimateFsm.GetInput(Input.AirQualityIsGettingGood);
                inputAirQualityIsGettingGood.TriggerMethod = AirQualityIsGettingGood;
                // Übergänge definieren
                IndoorClimateFsm.AddTransition(State.GoodAirQuality, State.BadAirQuality, Input.AirQualityIsGettingBad);
                IndoorClimateFsm.AddTransition(State.BadAirQuality, State.DangerousAirQuality, Input.AirQualityIsGettingDangerous);
                IndoorClimateFsm.AddTransition(State.BadAirQuality, State.GoodAirQuality, Input.AirQualityIsGettingGood);
                IndoorClimateFsm.AddTransition(State.DangerousAirQuality, State.BadAirQuality, Input.AirQualityIsGettingBetter);
                // Aktionen festlegen
                IndoorClimateFsm.GetState(State.DangerousAirQuality).OnEnter += StartAdministratorNotification;
                IndoorClimateFsm.GetState(State.DangerousAirQuality).OnLeave += StopAdministratorNotification;
                IndoorClimateFsm.Start(State.GoodAirQuality);
            }
            catch (Exception ex)
            {
                Log.Error($"Fehler bei Init FsmOilBurner, ex: {ex.Message}");
            }
        }

        #region TriggerMethoden

        public (bool, string) AirQualityIsGettingBad()
        {
            double co2 = StateService.GetLastMeasurement(ItemEnum.Seminar_Co2)?.Value ?? 0;
            var airQualityIsGettingBad = co2 >= BAD_AIR_QUALITY_CO2;
            return (airQualityIsGettingBad, $"AirQualityIsGettingBad; Co2: {co2}");
        }

        public (bool, string) AirQualityIsGettingDangerous()
        {
            double co2 = StateService.GetLastMeasurement(ItemEnum.Seminar_Co2)?.Value ?? 0;
            var airQualityIsGettingBad = co2 >= DANGEROUS_AIR_QUALITY_CO2;
            return (airQualityIsGettingBad, $"AirQualityIsGettingDangerous; Co2: {co2}");
        }

        public (bool, string) AirQualityIsGettingBetter()
        {
            double co2 = StateService.GetLastMeasurement(ItemEnum.Seminar_Co2)?.Value ?? 0;
            var airQualityIsGettingBad = co2 <= DANGEROUS_AIR_QUALITY_CO2 - HYSTERESE;
            return (airQualityIsGettingBad, $"AirQualityIsGettingBetter; Co2: {co2}");
        }

        public (bool, string) AirQualityIsGettingGood()
        {
            double co2 = StateService.GetLastMeasurement(ItemEnum.Seminar_Co2)?.Value ?? 0;
            var airQualityIsGettingBad = co2 <= BAD_AIR_QUALITY_CO2 - HYSTERESE;
            return (airQualityIsGettingBad, $"AirQualityIsGettingGood; Co2: {co2}");
        }

        #endregion


        #region Aktionen
        void StartAdministratorNotification(object? sender, EventArgs? e)
        {
            Log.Information($"IndoorClimateFsm;StartAdministratorNotification");
            Actor? alarmSwitch = StateService.GetActor(ItemEnum.IotShellyPlug_Relay);
            if (alarmSwitch != null)
            {
                alarmSwitch.SetActor(1);
            }
            else
            {
                Log.Error("RuleEngine;DoNotifyAdministrator;IotShellyPlug_Relay not defined");
            }
        }

        void StopAdministratorNotification(object? sender, EventArgs? e)
        {
            Log.Information($"IndoorClimateFsm;StopAdministratorNotification");
            Actor? alarmSwitch = StateService.GetActor(ItemEnum.IotShellyPlug_Relay);
            if (alarmSwitch != null)
            {
                alarmSwitch.SetActor(1);
            }
            else
            {
                Log.Error("RuleEngine;DoNotifyAdministrator;IotShellyPlug_Relay not defined");
            }
        }
        #endregion



    }
}
