using System;
using CIM.RemoteManager.Core.Helpers;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A CIMScan sensor.
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

        /// <summary>
        /// The sensor unit type (°C, %RH).
        /// </summary>
        public string SensorUnitType
        {
            get
            {
                // Validate
                if (!string.IsNullOrEmpty(_sensorType))
                {
                    // Return a sensor unit type
                    return _sensorType.GetSensorTypeResult().SensorUnitType;
                }
                // Default
                return "";
            }
        }

        /// <summary>
        /// Gets the sensor image based on the sensor type.
        /// </summary>
        /// <value>
        /// The sensor image source from resource directory.
        /// </value>
        public ImageSource SensorImage
        {
            get
            {
                // Validate
                if (!string.IsNullOrEmpty(_sensorType.GetSensorTypeResult().SensorImage))
                {
                    // Return a non-cached image source
                    return ImageSource.FromFile(_sensorType.GetSensorTypeResult().SensorImage);
                }
                // Default image if nothing found
                return ImageSource.FromFile("defaultSensor.png");
            }
        }

        private string _name = "N/A";
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_name))
                {
                    return _name;
                }
                // Default
                return "CIMScan Sensor";
            }
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// The sensor type group (Temperature, Differential Pressure, etc.).
        /// </summary>
        public string SensorTypeGroup
        {
            get
            {
                // Validate
                if (!string.IsNullOrEmpty(_sensorType))
                {
                    // Return a sensor unit type
                    return _sensorType.GetSensorTypeResult().SensorGroup;
                }
                // Default
                return "";
            }
        }

        /// <summary>
        /// The sensor type from the remote.  Used for converting to sensor
        /// labels, image, and grouping.
        /// </summary>
        private string _sensorType;
        public string SensorType
        {
            get => _sensorType;
            set => SetProperty(ref _sensorType, value);
        }

        /// <summary>
        /// Gets the sensor label.
        /// </summary>
        /// <value>
        /// The sensor label converted from sensor type.
        /// </value>
        public string SensorLabel
        {
            get
            {
                if (!string.IsNullOrEmpty(_sensorType))
                {
                    return _sensorType.GetSensorTypeResult().SensorLabel;
                }
                // Default
                return string.Empty;
            }
        }

        private double _scale;
        public double Scale
        {
            get
            {
                if (double.TryParse(_scale.ToString(), out double scaleValueResult))
                {
                    return scaleValueResult / 10000;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _scale, value);
        }

        private double _offset;
        public double Offset
        {
            get
            {
                if (double.TryParse(_offset.ToString(), out double offsetValueResult))
                {
                    return offsetValueResult / 10000;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _offset, value);
        }

        private int _timeStamp;
        public int TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime.
        /// </summary>
        public DateTime? DateTimeStamp => _timeStamp.UnixTimeStampToDateTime();

        /// <summary>
        /// Average sensor value plus the value unit type appended.
        /// </summary>
        public string AveragePlusUnitValue
        {
            get
            {
                if (!String.IsNullOrEmpty(AverageValue.ToString()))
                {
                    return $"{AverageValue.ToString()} {SensorUnitType}";
                }
                // Default
                return "No Value";
            }
        }

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _averageValue;
        public double AverageValue
        {
            get
            {
                if (double.TryParse(_averageValue.ToString(), out double averageValueResult))
                {
                    double finalResult = averageValueResult / 10;
                    if (Math.Abs(finalResult - 32768) < 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return finalResult;
                    }
                }
                // Default
                return 0;
            }
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