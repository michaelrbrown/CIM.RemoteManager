﻿namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public interface ISensor
    {
        int SensorIndex { get; set; }
        string SerialNumber { get; set; }
        string Name { get; set; }
        string SensorType { get; set; }
        double Scale { get; set; }
        double Offset { get; set; }
        int TimeStamp { get; set; }
        double AverageValue { get; set; }
        double CurrentValue { get; set; }
        int DisplayConversionCode { get; set; }
        int DecimalLocation { get; set; }
        string StatisticsTotalCalcSettings { get; set; }
    }
}