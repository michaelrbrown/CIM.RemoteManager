using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CIM.RemoteManager.Core.Extensions;
using CIM.RemoteManager.Core.Helpers;
using CIM.RemoteManager.Core.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;


namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorDetailsViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Bluetooth LE device
        /// </summary>
        private readonly IBluetoothLE _bluetoothLe;
        /// <summary>
        /// User dialogs
        /// </summary>
        private readonly IUserDialogs _userDialogs;
        /// <summary>
        /// Bluetooth LE Device
        /// </summary>
        private IDevice _device;
        /// <summary>
        /// Bluetooth LE Service
        /// </summary>
        private IService _service;

        /// <summary>
        /// Write characteristic
        /// </summary>
        public ICharacteristic TxCharacteristic { get; private set; }
        /// <summary>
        /// Read characteristic
        /// </summary>
        public ICharacteristic RxCharacteristic { get; private set; }

        /// <summary>
        /// Let our UI know we have updates started / stopped
        /// </summary>
        public bool UpdatesStarted { get; private set; }

        /// <summary>
        /// Is Bluetooth LE state on?
        /// </summary>
        public bool IsStateOn => _bluetoothLe.IsOn;

        /// <summary>
        /// Bluetooth LE states
        /// </summary>
        public string StateText => GetStateText();

        /// <summary>
        /// Convert our characteristics values from bytes to string as they are incoming
        /// </summary>
        public string CharacteristicValue => RxCharacteristic?.Value.BytesToStringConverted();

        /// <summary>
        /// Device name (from bluetooth name field)
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Sensor index selected from device (unique id)
        /// </summary>
        string _sensorIndexSelected = "00";
        public string SensorIndexSelected
        {
            get
            {
                // Try to lookup hex to string
                if (!String.IsNullOrEmpty(_sensorIndexSelected))
                {
                    return _sensorIndexSelected.GetSensorIndexFormatted();
                }
                // Default
                return _sensorIndexSelected;
            }
            set => SetProperty(ref _sensorIndexSelected, value);
        }

        /// <summary>
        /// Sensor index (unique id)
        /// </summary>
        public int SensorIndex { get; set; }

        /// <summary>
        /// Sensor
        /// </summary>
        public Sensor Sensor { get; set; }

        public enum SensorCommand
        {
            Plot,
            Statistics,
            Limits,
            Calibration
        };

        /// <summary>
        /// Sensor
        /// </summary>
        private SensorCommand _sensorCommand;
        public SensorCommand SensorCommandType
        {
            get => _sensorCommand;
            set => SetProperty(ref _sensorCommand, value);
        }

        /// <summary>
        /// Sensor collection
        /// </summary>
        FullyObservableCollection<ChartDataPoint> _sensorPlotCollection;
        public FullyObservableCollection<ChartDataPoint> SensorPlotCollection
        {
            get => _sensorPlotCollection;
            set => SetProperty(ref _sensorPlotCollection, value);
        }

        /// <summary>
        /// Show sensor updates mode
        /// </summary>
        public string UpdateButtonText => UpdatesStarted ? "Updates On" : "Updates Off";

        /// <summary>
        /// Is loading indicator for view
        /// </summary>
        bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Read try count before stopping characteristic service.  A fail safe.
        /// </summary>
        public int RxTryCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether [start unfiltered sensor value record].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [start unfiltered sensor value record]; otherwise, <c>false</c>.
        /// </value>
        public bool StartUnfilteredSensorValueRecord { get; set; } = false;
        /// <summary>
        /// Sensor record types (serialized into models later)
        /// </summary>
        public bool StartSensorBufferedValueRecord { get; set; } = false;
        /// <summary>
        /// Sensor record types (serialized into models later)
        /// </summary>
        public bool StartSensorStatisticsValueRecord { get; set; } = false;
        /// <summary>
        /// Sensor record types (serialized into models later)
        /// </summary>
        public bool StartSensorLimitValueRecord { get; set; } = false;
        /// <summary>
        /// Sensor record types (serialized into models later)
        /// </summary>
        public bool StartMessageCounterValueRecord { get; set; } = false;

        /// <summary>
        ///  "C" = unfiltered (current) value
        ///   # | time | current value
        /// </summary>
        public readonly StringBuilder UnfilteredSensorValue = new StringBuilder("");
        /// <summary>
        ///  "J" = buffered sensor data
        ///   number of points
        ///   time
        ///   value
        /// </summary>
        public readonly StringBuilder BufferedSensorValue = new StringBuilder("");
        /// <summary>
        ///  "H" = full information
        ///   index (#)
        ///   hourly maximum
        ///   timestamp for max
        ///   hourly minimum
        ///   timestamp for min
        ///   hourly average
        ///   hourly total
        /// </summary>
        public readonly StringBuilder StatisticsSensorValue = new StringBuilder("");
        /// <summary>
        ///  "F" = full information
        ///   total outgoing
        ///   retries
        ///   values
        ///   total incoming
        ///   errors
        ///   time since last message
        ///   active sensors
        ///   buffered measurements
        ///   current date/time
        /// </summary>
        public readonly StringBuilder MessageCounterValue = new StringBuilder("");

        #endregion

        #region Sensor

        /// <summary>
        /// Sensor name
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Sensor serial number (unique id)
        /// </summary>
        public string SensorSerialNumber { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _upperCalibration;
        public double UpperCalibration
        {
            get => _upperCalibration;
            set => SetProperty(ref _upperCalibration, value);
        }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _upperCalibrationTarget;
        public double UpperCalibrationTarget
        {
            get => _upperCalibrationTarget;
            set => SetProperty(ref _upperCalibrationTarget, value);
        }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _lowerCalibration;
        public double LowerCalibration
        {
            get => _lowerCalibration;
            set => SetProperty(ref _lowerCalibration, value);
        }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _lowerCalibrationTarget;
        public double LowerCalibrationTarget
        {
            get => _lowerCalibrationTarget;
            set => SetProperty(ref _lowerCalibrationTarget, value);
        }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _sensorScale;
        public double SensorScale
        {
            get => _sensorScale;
            set => SetProperty(ref _sensorScale, value);
        }

        /// <summary>
        /// Sensor Scale
        /// </summary>
        private double _sensorOffset;
        public double SensorOffset
        {
            get => _sensorOffset;
            set => SetProperty(ref _sensorOffset, value);
        }

        /// <summary>
        /// The current value
        /// </summary>
        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }

        /// <summary>
        /// Sensor timestamp
        /// </summary>
        private int _timeStamp;
        public int TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        #endregion

        #region Sensor Statistics

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _averageValue;
        public double AverageValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_averageValue.ToString(), out double averageValueResult))
                {
                    return averageValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _averageValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private int _sinceTimeStamp;
        public int SinceTimeStamp
        {
            get => _sinceTimeStamp;
            set => SetProperty(ref _sinceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        public DateTime? SinceDateTimeStamp => _sinceTimeStamp.UnixTimeStampToDateTime();


        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _minimumValue;
        public double MinimumValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_minimumValue.ToString(), out double minimumValueResult))
                {
                    return minimumValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _minimumValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private int _minimumOccuranceTimeStamp;
        public int MinimumOccuranceTimeStamp
        {
            get => _minimumOccuranceTimeStamp;
            set => SetProperty(ref _minimumOccuranceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        public DateTime? MinimumOccuranceDateTimeStamp => _minimumOccuranceTimeStamp.UnixTimeStampToDateTime();

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _maximumValue;
        public double MaximumValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_maximumValue.ToString(), out double maximumValueResult))
                {
                    return maximumValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _maximumValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private int _maximumOccuranceTimeStamp;
        public int MaximumOccuranceTimeStamp
        {
            get => _maximumOccuranceTimeStamp;
            set => SetProperty(ref _maximumOccuranceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        public DateTime? MaximumOccuranceDateTimeStamp => _maximumOccuranceTimeStamp.UnixTimeStampToDateTime();

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _varianceValue;
        public double VarianceValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_maximumValue.ToString(), out double maximumValueResult) && (double.TryParse(_minimumValue.ToString(), out double minimumValueResult)))
                {
                    return maximumValueResult - minimumValueResult;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _varianceValue, value);
        }

        #endregion

        #region Sensor Limits

        /// <summary>
        /// Alarm status.
        /// </summary>
        private int _alarmStatus;
        public int AlarmStatus
        {
            get => _alarmStatus;
            set => SetProperty(ref _alarmStatus, value);
        }

        /// <summary>
        /// Gets the alarm status for the sensor limits view.
        /// </summary>
        /// <value>
        /// The alarm status boolean value converted from
        /// integer.
        /// </value>
        public bool? AlarmStatusBool
        {
            get
            {
                if (!String.IsNullOrEmpty(_alarmStatus.ToString()))
                {
                    return Convert.ToBoolean(_alarmStatus);
                }
                // Default
                return null;
            }
        }

        /// <summary>
        /// Alarm being processed.
        /// </summary>
        private int _alarmBeingProcessed;
        public int AlarmBeingProcessed
        {
            get => _alarmBeingProcessed;
            set => SetProperty(ref _alarmBeingProcessed, value);
        }

        /// <summary>
        /// Alarm delay.
        /// </summary>
        private double _alarmDeley;
        public double AlarmDelay
        {
            get => _alarmDeley;
            set => SetProperty(ref _alarmDeley, value);
        }

        /// <summary>
        /// Low alarm limit.
        /// </summary>
        private double _lowAlarmLimit;
        public double LowAlarmLimit
        {
            get => _lowAlarmLimit;
            set => SetProperty(ref _lowAlarmLimit, value);
        }

        /// <summary>
        /// Low warning limit.
        /// </summary>
        private double _lowWarningLimit;
        public double LowWarningLimit
        {
            get => _lowWarningLimit;
            set => SetProperty(ref _lowWarningLimit, value);
        }

        /// <summary>
        /// High alarm limit.
        /// </summary>
        private double _highAlarmLimit;
        public double HighAlarmLimit
        {
            get => _highAlarmLimit;
            set => SetProperty(ref _highAlarmLimit, value);
        }

        /// <summary>
        /// High warning limit.
        /// </summary>
        private double _highWarningLimit;
        public double HighWarningLimit
        {
            get => _highWarningLimit;
            set => SetProperty(ref _highWarningLimit, value);
        }

        #endregion

        #region Message Counters

        private int _totalOutgoingMessages;
        public int TotalOutgoingMessages
        {
            get => _totalOutgoingMessages;
            set => SetProperty(ref _totalOutgoingMessages, value);
        }

        private int _totalOutgoingRetries;
        public int TotalOutgoingRetries
        {
            get => _totalOutgoingRetries;
            set => SetProperty(ref _totalOutgoingRetries, value);
        }

        private int _totalOutgoingValues;
        public int TotalOutgoingValues
        {
            get => _totalOutgoingValues;
            set => SetProperty(ref _totalOutgoingValues, value);
        }

        private int _totalIncomingMessages;
        public int TotalIncomingMessages
        {
            get => _totalIncomingMessages;
            set => SetProperty(ref _totalIncomingMessages, value);
        }

        private int _totalIncomingErrors;
        public int TotalIncomingErrors
        {
            get => _totalIncomingErrors;
            set => SetProperty(ref _totalIncomingErrors, value);
        }

        /// <summary>
        /// Time in minutes
        /// </summary>
        private int _lastServerMessageReceived;
        public int LastServerMessageReceived
        {
            get => _lastServerMessageReceived;
            set => SetProperty(ref _lastServerMessageReceived, value);
        }

        /// <summary>
        /// Total number of sensors attached to remote
        /// </summary>
        private int _totalActiveSensors;
        public int TotalActiveSensors
        {
            get => _totalActiveSensors;
            set => SetProperty(ref _totalActiveSensors, value);
        }

        /// <summary>
        /// Number of records in history buffer (this value is divided by 256)
        /// </summary>
        private int _totalRecordsInHistoryBuffer;
        public int TotalRecordsInHistoryBuffer
        {
            get
            {
                // Try to lookup hex to string
                if (int.TryParse(_totalRecordsInHistoryBuffer.ToString(), out int totalRecordsInHistoryBufferResult))
                {
                    return totalRecordsInHistoryBufferResult / 256;
                }
                // Default
                return _totalRecordsInHistoryBuffer;
            }
            set => SetProperty(ref _totalRecordsInHistoryBuffer, value);
        }

        /// <summary>
        /// Present Unix time
        /// </summary>
        private int _currentDateTime;
        public int CurrentDateTime
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }

        #endregion

        #region Parsing Routines

        /// <summary>
        /// Get unfiltered (current) values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private async Task GetUnfilteredSensorValuesAsync(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "unfiltered (current) sensor values"
            if (!StartUnfilteredSensorValueRecord && characteristicValue.Contains("{F"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredSensorValue.Append(characteristicValue);
                    await SerializeStringToSensorAsync(UnfilteredSensorValue.ToString(), "C");
                    UnfilteredSensorValue.Clear();
                    StartUnfilteredSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    UnfilteredSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartUnfilteredSensorValueRecord = true;
                }
            }
            else if (StartUnfilteredSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    await SerializeStringToSensorAsync(UnfilteredSensorValue.ToString(), "C");
                    UnfilteredSensorValue.Clear();
                    StartUnfilteredSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    UnfilteredSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Get message counters from remote.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private async Task GetMessageCounterValuesAsync(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "message counter values"
            if (!StartMessageCounterValueRecord && characteristicValue.Contains("{F"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    MessageCounterValue.Append(characteristicValue);
                    await SerializeStringToSensorAsync(MessageCounterValue.ToString(), "F");
                    MessageCounterValue.Clear();
                    StartMessageCounterValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    MessageCounterValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartMessageCounterValueRecord = true;
                }
            }
            else if (StartMessageCounterValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    MessageCounterValue.Append(characteristicValue.GetUntilOrEmpty());
                    await SerializeStringToSensorAsync(MessageCounterValue.ToString(), "F");
                    MessageCounterValue.Clear();
                    StartMessageCounterValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    MessageCounterValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Get sensor plot (buffered) data from remote.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private async Task GetSensorPlotValuesAsync(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor plot values"
            if (!StartSensorBufferedValueRecord && characteristicValue.Contains("{J"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    BufferedSensorValue.Append(characteristicValue);
                    await SerializeStringToSensorAsync(BufferedSensorValue.ToString());
                    BufferedSensorValue.Clear();
                    StartSensorBufferedValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    BufferedSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartSensorBufferedValueRecord = true;
                }
            }
            else if (StartSensorBufferedValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    BufferedSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    await SerializeStringToSensorAsync(BufferedSensorValue.ToString());
                    BufferedSensorValue.Clear();
                    StartSensorBufferedValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    BufferedSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Get sensor statistics values from remote.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private async Task GetSensorStatisticsValuesAsync(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor statistics values"
            if (!StartSensorStatisticsValueRecord && characteristicValue.Contains("{H"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue);
                    await SerializeStringToSensorAsync(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartSensorStatisticsValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartSensorStatisticsValueRecord = true;
                }
            }
            else if (StartSensorStatisticsValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    await SerializeStringToSensorAsync(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartSensorStatisticsValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Get sensor limits values from remote.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private async Task GetSensorLimitsValuesAsync(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor limits values"
            if (!StartSensorLimitValueRecord && characteristicValue.Contains("{G"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue);
                    await SerializeStringToSensorAsync(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartSensorLimitValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartSensorLimitValueRecord = true;
                }
            }
            else if (StartSensorLimitValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    await SerializeStringToSensorAsync(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartSensorLimitValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Serialize tab based sensor data to strongly typed Sensor model.
        /// </summary>
        /// <param name="sensorValues"></param>
        /// <param name="conversionType"></param>
        private async Task SerializeStringToSensorAsync(string sensorValues, string conversionType = "")
        {
            // Split by tab delimiter
            string[] splitSensorValues = sensorValues.Split('\t');

            try
            {
                switch (conversionType)
                {
                    case "C":
                        //Application.Current.MainPage.DisplayAlert("C", splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0).ToString(), "Cancel");

                        // "C" Sensor data serialization
                        // Update Sensor list by index
                        if (SensorIndexSelected.GetSensorIndexAsInt() == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('C') + 1).SafeConvert<int>(0))
                        {
                            //Application.Current.MainPage.DisplayAlert("Old value: ", sensorListItem.AverageValue.ToString(), "Cancel");
                            //await Application.Current.MainPage.DisplayAlert("(C) CurrentValue: ", splitSensorValues[1].SafeHexToDouble().ToString(), "Cancel");

                            _userDialogs.InfoToast($"(C) CurrentValue: {splitSensorValues[1].SafeHexToDouble().ToString()}", TimeSpan.FromSeconds(5));

                            TimeStamp = splitSensorValues[0].SafeHexToInt();
                            CurrentValue = splitSensorValues[1].SafeHexToDouble();
                        }
                        break;
                    case "F":
                        // "F" Message counter data serialization
                        TotalOutgoingMessages = sensorValues.Substring(1, 2).SafeHexToInt();
                        TotalOutgoingRetries = sensorValues.Substring(3, 2).SafeHexToInt();
                        TotalOutgoingValues = sensorValues.Substring(5, 2).SafeHexToInt();
                        TotalIncomingMessages = sensorValues.Substring(7, 2).SafeHexToInt();
                        TotalIncomingErrors = sensorValues.Substring(9, 2).SafeHexToInt();
                        LastServerMessageReceived = sensorValues.Substring(11, 2).SafeHexToInt();
                        TotalActiveSensors = sensorValues.Substring(13, 2).SafeHexToInt();
                        TotalRecordsInHistoryBuffer = sensorValues.Substring(15, 2).SafeHexToInt();
                        CurrentDateTime = sensorValues.Substring(19, 8).SafeHexToInt();

                        //_userDialogs.Alert($"(F) Message Counters Data: {sensorValues}", "CIMScan RemoteManager");

                        //_userDialogs.Alert($"(F) Message Counters TotalActiveSensors: {TotalActiveSensors}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(F) Message Counters CurrentDateTime: {CurrentDateTime}", "CIMScan RemoteManager");

                        break;
                    default:
                        if (SensorCommandType == SensorCommand.Plot)
                        {
                            //_userDialogs.Alert($"(J) Buffered Data: {sensorValues}", "CIMScan RemoteManager");

                            // New instance of sensor plot
                            var sensorPlot = new SensorPlot();

                            //SensorPlotCollection.Add(new ChartDataPoint("Jan", 42));
                            //SensorPlotCollection.Add(new ChartDataPoint("Feb", 44));
                            //SensorPlotCollection.Add(new ChartDataPoint("Mar", 53));
                            //SensorPlotCollection.Add(new ChartDataPoint("Apr", 64));
                            //SensorPlotCollection.Add(new ChartDataPoint("May", 75));
                            //SensorPlotCollection.Add(new ChartDataPoint("Jun", 83));
                            //SensorPlotCollection.Add(new ChartDataPoint("Jul", 87));
                            //SensorPlotCollection.Add(new ChartDataPoint("Aug", 84));
                            //SensorPlotCollection.Add(new ChartDataPoint("Sep", 78));
                            //SensorPlotCollection.Add(new ChartDataPoint("Oct", 67));
                            //SensorPlotCollection.Add(new ChartDataPoint("Nov", 55));
                            //SensorPlotCollection.Add(new ChartDataPoint("Dec", 45));

                            //return;


                            // Defaults
                            int plotIndex = 1;
                            bool plotTime = true;

                            //await Application.Current.MainPage.DisplayAlert("(J) 1st val: ", splitSensorValues[0].ToString(), "Cancel").ConfigureAwait(true);

                            // Get number of plot points.
                            // Multiply times two since we have to collect time and value.
                            int numberOfPlotPoints = splitSensorValues[0].Substring(4, splitSensorValues[0].Length).SafeHexToInt() * 2;

                            //_userDialogs.Alert($"(J) sensorPlot.numberOfPlotPoints: {numberOfPlotPoints.ToString()}", "CIMScan RemoteManager");

                            await Application.Current.MainPage.DisplayAlert("(J) numberOfPlotPoints: ", numberOfPlotPoints.ToString(), "Cancel").ConfigureAwait(true);

                            //_userDialogs.InfoToast($"(J) sensorPlot.numberOfPlotPoints: {numberOfPlotPoints.ToString()}", TimeSpan.FromSeconds(5));


                            // Iterate through plot values and set plot datetime and current value
                            for (int i = 1; i <= numberOfPlotPoints; i++)
                            {
                                if (plotTime)
                                {
                                    // Plot time
                                    sensorPlot.UnixTimeStamp = splitSensorValues[i].SafeHexToInt();
                                    sensorPlot.TimeStamp = sensorPlot.UnixTimeStamp.UnixTimeStampToDateTime();
                                    plotTime = false;

                                    //_userDialogs.Alert($"(J) sensorPlot.TimeStamp: {sensorPlot.TimeStamp}", "CIMScan RemoteManager");
                                }
                                else
                                {
                                    // Plot value
                                    sensorPlot.CurrentValue = splitSensorValues[i].SafeHexToInt();
                                    plotTime = true;

                                    //_userDialogs.Alert($"(J) sensorPlot.CurrentValue: {sensorPlot.CurrentValue}", "CIMScan RemoteManager");
                                }

                                // Every two iterations add values to chart collection
                                if ((plotIndex % 2) == 0)
                                {
                                    await Application.Current.MainPage.DisplayAlert("(J) add data point: ", sensorPlot.TimeStamp.ToString("HH:mm") + "-" + sensorPlot.CurrentValue.ToString(), "Cancel").ConfigureAwait(true);
                                    // Add plot data to list
                                    SensorPlotCollection.Add(new ChartDataPoint(sensorPlot.TimeStamp.ToString("HH:mm"), sensorPlot.CurrentValue));
                                }
                                plotIndex++;
                            }


                            await Task.Delay(2000).ConfigureAwait(true);
                            // Send a refresh command after we load plot data to refresh other page data
                            await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(true);

                        }
                        else if (SensorCommandType == SensorCommand.Statistics)
                        {
                            //_userDialogs.Alert($"(H) Statistics Data: {sensorValues}", "CIMScan RemoteManager");
                            //_userDialogs.Alert($"(H) SensorIndexSelected: {SensorIndexSelected}", "CIMScan RemoteManager");
                            //_userDialogs.Alert($"(H) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                            // Only update the values if we have a match
                            if (SensorIndexSelected.GetSensorIndexAsInt() == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0))
                            {
                                _userDialogs.Alert($"(H) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                                // "H" Sensor data serialization
                                MaximumValue = splitSensorValues[1].SafeHexToDouble();
                                MaximumOccuranceTimeStamp = splitSensorValues[2].SafeHexToInt();
                                MinimumValue = splitSensorValues[3].SafeHexToDouble();
                                MinimumOccuranceTimeStamp = splitSensorValues[4].SafeHexToInt();
                                AverageValue = splitSensorValues[5].SafeHexToDouble();
                                SinceTimeStamp = splitSensorValues[6].SafeHexToInt();
                            }
                        }
                        else if (SensorCommandType == SensorCommand.Limits)
                        {
                            //_userDialogs.Alert($"(G) Limits Data: {sensorValues}", "CIMScan RemoteManager");
                            //_userDialogs.Alert($"(G) SensorIndexSelected: {SensorIndexSelected}", "CIMScan RemoteManager");
                            //_userDialogs.Alert($"(G) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('G') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                            // Only update the values if we have a match
                            if (SensorIndexSelected.GetSensorIndexAsInt() == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('G') + 1).SafeConvert<int>(0))
                            {
                                _userDialogs.Alert($"(G) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('G') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                                // "G" Sensor data serialization
                                AlarmStatus = splitSensorValues[1].SafeHexToInt();
                                //AlarmBeingProcessed = splitSensorValues[2].SafeHexToInt();
                                AlarmDelay = splitSensorValues[2].SafeHexToDouble();
                                LowAlarmLimit = splitSensorValues[3].SafeHexToInt();
                                LowWarningLimit = splitSensorValues[4].SafeHexToDouble();
                                HighWarningLimit = splitSensorValues[5].SafeHexToInt();
                                HighAlarmLimit = splitSensorValues[6].SafeHexToDouble();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(SerializeStringToSensor) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                //await Application.Current.MainPage.DisplayAlert("CIMScan", splitSensorValues.ToString(), "Cancel");
                //_userDialogs.Alert($"(SerializeStringToSensor) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event to handle Bluetooth LE state changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            RaisePropertyChanged(() => IsStateOn);
            RaisePropertyChanged(() => StateText);
        }

        /// <summary>
        /// Event to handle Bluetooth connection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            if (UpdatesStarted)
            {
                StopUpdates();
            }
        }

        /// <summary>
        /// Rx characteristic updated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="characteristicUpdatedEventArgs"></param>
        private async void RxCharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            try
            {
                // Get data based on tab selected
                if (SensorCommandType == SensorCommand.Plot)
                {
                    await GetSensorPlotValuesAsync(CharacteristicValue);
                }
                else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                {
                    await GetSensorStatisticsValuesAsync(CharacteristicValue);
                    await GetSensorLimitsValuesAsync(CharacteristicValue);
                }

                // Get unfiltered (current) sensor values
                await GetUnfilteredSensorValuesAsync(CharacteristicValue);
                // Get message counter values from remote to determine if
                // we have acquired or lost sensors.  Also grabs time stamp.
                //GetMessageCounterValues(CharacteristicValue);

                // Notify property changed
                RaisePropertyChanged(() => CharacteristicValue);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(CharacteristicOnValueUpdated) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Start sensor updates.
        /// </summary>
        public MvxCommand StartUpdatesCommand => new MvxCommand(() =>
        {
            if (!UpdatesStarted)
            {
                StartUpdates();
            }
        });

        /// <summary>
        /// Stop sensor updates.
        /// </summary>
        public MvxCommand StopUpdatesCommand => new MvxCommand(() =>
        {
            if (UpdatesStarted)
            {
                StopUpdates();
            }
        });

        /// <summary>
        /// Toggle sensor updates.
        /// </summary>
        public MvxCommand ToggleUpdatesCommand => new MvxCommand(() =>
        {
            if (UpdatesStarted)
            {
                StopUpdates();
            }
            else
            {
                StartUpdates();
            }
        });

        /// <summary>
        /// Save sensor calibration offset and scale.
        /// </summary>
        public MvxCommand SaveSensorCalibration => new MvxCommand(SaveSensorCalibrationData);

        #endregion

        /// <summary>
        /// Sensor plot view model constructor.
        /// </summary>
        /// <param name="bluetoothLe">Bluetooth LE obj</param>
        /// <param name="adapter">Bluetooth LE adapter</param>
        /// <param name="userDialogs">User dialogs</param>
        public SensorDetailsViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            try
            {
                _bluetoothLe = bluetoothLe;
                _userDialogs = userDialogs;

                // Events
                _bluetoothLe.StateChanged += OnStateChanged;

                // Register event for device connection lost
                Adapter.DeviceConnectionLost += OnDeviceConnectionLost;

                // Sensor data
                _sensorPlotCollection = new FullyObservableCollection<ChartDataPoint>();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
                Mvx.Trace(ex.Message);
            }
        }

        /// <summary>
        /// Initialization of bluetooth service characteristics.
        /// Refresh command sent to remote to start sensor data flow.
        /// Reading in of A, B, and F sensor records and serializing
        /// to model for display in UI.
        /// </summary>
        private async void InitRemote()
        {
            // Validate
            if (_device == null)
            {
                throw new ArgumentNullException(nameof(_device));
            }

            try
            {
                // Loading indicator
                IsLoading = true;

                // Get our Adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid).ConfigureAwait(true);

                // Get write characteristic service
                TxCharacteristic = await _service.GetCharacteristicAsync(TxUuid).ConfigureAwait(true);

                //// Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    // Setup plot command
                    string updateValue = string.Empty;
                    //if (SensorCommandType == SensorCommand.Plot)
                    //{
                        // Plot 10 points
                        updateValue = "{c0" + SensorIndexSelected + "0000000A}";
                    //}
                    // Send statistics, and limits commands (Y command = refresh all which returns
                    // limits and statistics data.
                    //else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                    //{
                    //updateValue = "{Y}";
                    //}

                    // TODO: remove after debugging
                    //_userDialogs.Alert($"Write Command: {updateValue}", "CIMScan Remote Manager");


                    //await Application.Current.MainPage.DisplayAlert("J Byte Array to Str: ", updateValue.StrToByteArray().ToString(), "Cancel");


                    // Send the command based on command type set above
                    await TxCharacteristic.WriteAsync(updateValue.StrToByteArray()).ConfigureAwait(true);
                }
                else
                {
                    _userDialogs.Alert("Cannot write characteristic data to remote!", "CIMScan Remote Manager");
                }

                // Get Characteristics service
                RxCharacteristic = await _service.GetCharacteristicAsync(RxUuid).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                HockeyApp.MetricsManager.TrackEvent($"(InitRemote) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(InitRemote) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
            finally
            {
                // Loading indicator (make sure it turns off even with exception thrown)
                IsLoading = false;
            }
        }

        /// <summary>
        /// MVVMCross init from bundle call from previous page.
        /// Set's up bundle params for initialization.
        /// </summary>
        /// <param name="parameters"></param>
        protected override void InitFromBundle(IMvxBundle parameters)
        {
            try
            {
                base.InitFromBundle(parameters);

                // Get selected sensor index from device
                SensorIndexSelected = parameters.Data[SensorIdKey];
                // Notify property changed
                RaisePropertyChanged(() => SensorIndexSelected);

                // TODO: remove after debugging
                //_userDialogs.Alert($"(InitFromBundle) SensorIndexSelected: {SensorIndexSelected.ToString()}");

                // Get sensor name from app context
                if (Application.Current.Properties.ContainsKey("CurrentSensorName"))
                {
                    SensorName = Convert.ToString(Application.Current.Properties["CurrentSensorName"]);
                    // Notify property changed
                    RaisePropertyChanged(() => SensorName);
                }
                // Get sensor type from app context
                if (Application.Current.Properties.ContainsKey("CurrentSensorType"))
                {
                    SensorType = Convert.ToString(Application.Current.Properties["CurrentSensorType"]);
                    // Notify property changed
                    RaisePropertyChanged(() => SensorType);
                }
                // Get sensor offset from app context
                if (Application.Current.Properties.ContainsKey("CurrentSensorOffset"))
                {
                    SensorOffset = Convert.ToDouble(Application.Current.Properties["CurrentSensorOffset"]);
                    // Notify property changed
                    RaisePropertyChanged(() => SensorOffset);
                }
                // Get sensor scale from app context
                if (Application.Current.Properties.ContainsKey("CurrentSensorScale"))
                {
                    SensorScale = Convert.ToDouble(Application.Current.Properties["CurrentSensorScale"]);
                    // Notify property changed
                    RaisePropertyChanged(() => SensorScale);
                }

                // Get device from bundle
                _device = GetSensorDeviceBundle(parameters);

                // Set device name
                DeviceName = _device.Name;

                // Init our DA-12
                InitRemote();

                // Dispose
                if (_device == null)
                {
                    Close(this);
                }
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(InitFromBundle) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(InitFromBundle) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// On Resume.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            if (!UpdatesStarted)
            {
                //StartUpdates();
            }
        }

        /// <summary>
        /// Setup all the possible Bluetooth LE states.
        /// </summary>
        /// <returns></returns>
        private string GetStateText()
        {
            switch (_bluetoothLe.State)
            {
                case BluetoothState.Unknown:
                    return "Unknown Bluetooth LE state.";
                case BluetoothState.Unavailable:
                    return "Bluetooth LE is not available on this device.";
                case BluetoothState.Unauthorized:
                    return "You are not allowed to use Bluetooth LE.";
                case BluetoothState.TurningOn:
                    return "Bluetooth LE is warming up, please wait...";
                case BluetoothState.On:
                    return "Bluetooth LE is on.";
                case BluetoothState.TurningOff:
                    return "Bluetooth LE is turning off...";
                case BluetoothState.Off:
                    return "Bluetooth LE is off. Please enable on your device.";
                default:
                    return "Unknown Bluetooth LE state.";
            }
        }

        /// <summary>
        /// Start Bluetooth characteristics updating.
        /// </summary>
        /// <remarks>
        /// We handle exceptions of trying to write commands to early to the remote
        /// by a recursive method which ensures our loading (init) process is complete
        /// before we start trying to handle updates (write commands before initialized).
        /// </remarks>
        private async void StartUpdates()
        {
            try
            {
                if (IsLoading)
                {
                    // Make sure we are done with our initialization before starting updates
                    while (IsLoading && !UpdatesStarted && RxTryCount < 100)
                    {
                        if (!IsLoading)
                        {
                            // Handle updates started
                            await HandleUpdatesStarted().ConfigureAwait(false);

                            // Reset try count
                            RxTryCount = 0;
                        }
                        else
                        {
                            await Task.Delay(1000).ConfigureAwait(true);
                            // Recursive process until we complete initialization
                            StartUpdates();
                        }
                        // Increment try count
                        RxTryCount++;
                    }
                }
                else
                {
                    // Initialization is done so let's just start
                    await HandleUpdatesStarted();
                }
            }
            catch (Exception ex)
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                HockeyApp.MetricsManager.TrackEvent($"(StartUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(StartUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Handle updates started by updating interface with bound values from parsed characteristics.
        /// </summary>
        /// <returns></returns>
        private async Task HandleUpdatesStarted()
        {
            try
            {
                UpdatesStarted = true;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                // Send plot or refresh command to remote
                string updateValue = string.Empty;
                if (SensorCommandType == SensorCommand.Plot)
                {
                    // Plot 10 points
                    updateValue = "{c0" + SensorIndexSelected + "0000000A}";
                }
                else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                {
                    //updateValue = "{Y}";
                }

                // Send a refresh command
                await TxCharacteristic.WriteAsync(updateValue.StrToByteArray()).ConfigureAwait(true);
                // Start updates from bluetooth service
                await RxCharacteristic.StartUpdatesAsync().ConfigureAwait(true);

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                RxCharacteristic.ValueUpdated += RxCharacteristicOnValueUpdated;

                // Let UI know mode we are in
                RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                HockeyApp.MetricsManager.TrackEvent($"(HandleUpdatesStarted) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(HandleUpdatesStarted) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Stop bluetooth characteristics updating.
        /// </summary>
        private async void StopUpdates()
        {
            try
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                // Stop updates from bluetooth service
                await RxCharacteristic.StopUpdatesAsync().ConfigureAwait(true);

                // Unsubscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;

                // Let UI know mode we are in
                RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(StopUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(StopUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Saves the sensor calibration data.
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        private async void SaveSensorCalibrationData()
        {
            try
            {
                UpdatesStarted = true;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                // Setup sensor scale command
                string sensorScaleUpdateValue = "{C" +  SensorIndexSelected + "\t" + Convert.ToInt32(SensorScale * 10000) + "}";

                _userDialogs.Alert($"sensorScaleUpdateValue: {sensorScaleUpdateValue}");

                _userDialogs.Alert($"sensorScaleUpdateValue byte: {sensorScaleUpdateValue.StrToByteArray().ToString()}");


                // Save scale and offset to remote
                await TxCharacteristic.WriteAsync(sensorScaleUpdateValue.StrToByteArray()).ConfigureAwait(true);

                // Setup sensor offset command
                string sensorOffsetUpdateValue = "{D" + SensorIndexSelected + "\t" + SensorOffset + "}";

                //_userDialogs.Alert($"sensorOffsetUpdateValue: {sensorScaleUpdateValue}");

                // Save scale and offset to remote
                await TxCharacteristic.WriteAsync(sensorOffsetUpdateValue.StrToByteArray()).ConfigureAwait(true);

                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                // Show complete
                _userDialogs.InfoToast($"Calibration was saved.", TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                HockeyApp.MetricsManager.TrackEvent($"(SaveSensorCalibrationData) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert($"(SaveSensorCalibrationData) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

    }
}