using System;

namespace Core.DataTransferObjects
{
    public record MeasurementGetDto(string ItemName, DateTime Time, double Value);

}
