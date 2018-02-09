namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public class Sensor : BindableBase
    {
        private double _averageValue;
        public int SensorIndex { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string SensorType { get; set; }
        public double Scale { get; set; }
        public double Offset { get; set; }
        public int TimeStamp { get; set; }

        public double AverageValue
        {
            get => _averageValue;
            set
            {
                _averageValue = value;
                OnPropertyChanged(nameof(AverageValue));
            }
        }

        public double CurrentValue { get; set; }
        public int DisplayConversionCode { get; set; }
        public int DecimalLocation { get; set; }
        public string StatisticsTotalCalcSettings { get; set; }
    }
}