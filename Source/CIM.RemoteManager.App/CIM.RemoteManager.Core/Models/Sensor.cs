namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public class Sensor : ISensor
    {
        public int Index { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string SensorType { get; set; }
        public decimal Scale { get; set; }
        public decimal Offset { get; set; }
        public int TimeStamp { get; set; }
        public decimal AverageValue { get; set; }
        public decimal CurrentValue { get; set; }
        public int DisplayConversionCode { get; set; }
        public int DecimalLocation { get; set; }
        public string StatisticsTotalCalcSettings { get; set; }
    }
}