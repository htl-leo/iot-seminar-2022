using System;

namespace Core.DataTransferObjects
{
    public record MeasurementDto(string ItemName, DateTime Time, double Value);
    public record MeasurementTimeValueDto(DateTime Time, double Value);

}
