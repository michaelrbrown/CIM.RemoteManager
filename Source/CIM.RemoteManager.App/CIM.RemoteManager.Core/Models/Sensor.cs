namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public class Sensor : BindableBase
    {
       
        private int _sensorIndex;
        public int SensorIndex
        {
            get => _sensorIndex;
            set => SetProperty(ref _sensorIndex, value);
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _sensorType;
        public string SensorType
        {
            get
            {
                // Try to lookup hex to string
                if (!string.IsNullOrEmpty(_sensorType))
                {
                    return _sensorType.LookupNameByValue();
                }
                // Default
                return string.Empty;
            }
            set => SetProperty(ref _sensorType, value);
        }

        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        private double _offset;
        public double Offset
        {
            get => _offset;
            set => SetProperty(ref _offset, value);
        }

        private double _timeStamp;
        public double TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }
        
        private double _averageValue;
        public double AverageValue
        {
            get => _averageValue;
            set => SetProperty(ref _averageValue, value);
        }

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }

        private int _displayConversionCode;
        public int DisplayConversionCode
        {
            get => _displayConversionCode;
            set => SetProperty(ref _displayConversionCode, value);
        }
        
        private int _decimalLocation;
        public int DecimalLocation
        {
            get => _decimalLocation;
            set => SetProperty(ref _decimalLocation, value);
        }

        private string _statisticsTotalCalcSettings;
        public string StatisticsTotalCalcSettings
        {
            get => _statisticsTotalCalcSettings;
            set => SetProperty(ref _statisticsTotalCalcSettings, value);
        }

        private int _alarmStatus;
        public int AlarmStatus
        {
            get => _alarmStatus;
            set => SetProperty(ref _alarmStatus, value);
        }

    }
}