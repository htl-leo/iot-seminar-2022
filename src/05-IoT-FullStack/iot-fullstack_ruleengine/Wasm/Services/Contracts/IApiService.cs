using System.Threading.Tasks;
using Base.DataTransferObjects;

using Core.Entities;
using Core.DataTransferObjects;
using System;
using IotServices.DataTransferObjects;

namespace Wasm.Services.Contracts
{
    public interface IApiService
    {
        Task<bool> ChangeSwitchAsync(string name, bool on);

        Task<IotServices.DataTransferObjects.MeasurementTimeValue[]> GetMeasurementsAsync(string sensorName, DateTime date);

    }
}
