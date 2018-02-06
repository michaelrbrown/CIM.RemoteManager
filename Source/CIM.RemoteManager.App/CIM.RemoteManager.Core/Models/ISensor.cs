namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public interface ISensor
    {
        int Index { get; set; }
        string SerialNumber { get; set; }
        string Name { get; set; }
        string SensorType { get; set; }
        decimal Scale { get; set; }
        decimal Offset { get; set; }
        int TimeStamp { get; set; }
        decimal AverageValue { get; set; }
        decimal CurrentValue { get; set; }
        int DisplayConversionCode { get; set; }
        int DecimalLocation { get; set; }
        string StatisticsTotalCalcSettings { get; set; }
    }
}