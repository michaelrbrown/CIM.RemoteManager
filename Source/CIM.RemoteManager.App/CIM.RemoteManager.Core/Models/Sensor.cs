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
        public double Scale { get; set; }
        public double Offset { get; set; }
        public int TimeStamp { get; set; }
        public double AverageValue { get; set; }
        public double CurrentValue { get; set; }
        public int DisplayConversionCode { get; set; }
        public int DecimalLocation { get; set; }
        public string StatisticsTotalCalcSettings { get; set; }
    }
}