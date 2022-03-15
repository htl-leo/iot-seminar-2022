using Core.Entities;

using Microsoft.AspNetCore.Components;

using System;
using System.Linq;
using System.Threading.Tasks;

using Wasm.DataTransferObjects;
using Wasm.Services.Contracts;

namespace Wasm.Pages.IoT
{
    public partial class ChartSensorValues
    {
        [Inject]
        public IApiService ApiService { get; set; }

        public string[] Items { get; private set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string SelectedItem { get; set; }
        public ChartDataItem[] DataItems { get; set; } = Array.Empty<ChartDataItem>();

        protected override async Task OnInitializedAsync()
        {
            string[] itemNames = new string[]
            {
                ItemEnum.Seminar_Co2.ToString(),
                ItemEnum.Seminar_Temperature.ToString(),
                ItemEnum.Seminar_Humidity.ToString(),
                ItemEnum.Seminar_Rssi.ToString(),
                ItemEnum.IotShellyPlug_Power.ToString(),
                ItemEnum.IotShellyPlug_Relay.ToString(),
            };
            Items = itemNames;
            SelectedItem = Items[0];
            await GetAndFillDataItemsAsync();
        }

        /// <summary>
        /// DataItems eines Tages für das Chart aufbereiten
        /// </summary>
        /// <returns></returns>
        private async Task GetAndFillDataItemsAsync()
        {
            Console.WriteLine($"GetAndFillDataItemsAsync for Sensor {SelectedItem} and Date: {SelectedDate.ToShortDateString()}");
            var measurements = await ApiService.GetMeasurementsAsync(SelectedItem, SelectedDate.Date);
            DataItems = measurements
                .Select(m => new ChartDataItem
                {
                   Time = m.Time,
                   QuarterOfAnHourNumber = (m.Time.Hour*60+m.Time.Minute)/15,
                   Value = m.Value
                })
                .ToArray();
            Console.WriteLine($"Chart mit {DataItems.Length} dataitems for chart");
        }

        /// <summary>
        /// X-Achse beschriften
        /// </summary>
        /// <param name="timeObject"></param>
        /// <returns></returns>
        public static string GetCategoryText(object timeObject)
        {
            var time = (DateTime)timeObject;
            return time.ToString("HH:mm");
        }

        /// <summary>
        /// Neues Item ausgewählt
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async Task OnChangeItem(string item)
        {
            SelectedItem = item;
            await GetAndFillDataItemsAsync();
        }

        /// <summary>
        /// Neues Datum ausgewählt
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected async Task OnChangeDate(string date)
        {
            SelectedDate = DateTime.Parse(date);
            await GetAndFillDataItemsAsync();
        }

    }
}

