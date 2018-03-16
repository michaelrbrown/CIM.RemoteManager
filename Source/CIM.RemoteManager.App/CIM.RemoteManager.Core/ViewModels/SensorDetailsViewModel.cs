using System;
using System.Text;
using Acr.UserDialogs;
using CIM.RemoteManager.Core.Helpers;
using CIM.RemoteManager.Core.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
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
        public bool UpdatesStarted;

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
        public int SensorIndexSelected { get; set; }

        /// <summary>
        /// Sensor index (unique id)
        /// </summary>
        public int SensorIndex { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        public string SensorName { get; set; }

        /// <summary>
        /// Sensor serial number (unique id)
        /// </summary>
        public string SensorSerialNumber { get; set; }

        /// <summary>
        /// Sensor
        /// </summary>
        public Sensor Sensor { get; set; }

        public enum SensorCommand
        {
            Plot,
            Statistics,
            Limits
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
        FullyObservableCollection<SensorPlot> _sensorPlotCollection;
        public FullyObservableCollection<SensorPlot> SensorPlotCollection
        {
            get => _sensorPlotCollection;
            set => SetProperty(ref _sensorPlotCollection, value);
        }

        /// <summary>
        /// Show sensor updates mode
        /// </summary>
        public string UpdateButtonText => UpdatesStarted ? "Updates On" : "Updates Off";

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
        private double _sinceTimeStamp;
        public double TimeStamp
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
        private double _minimumOccuranceTimeStamp;
        public double MinimumOccuranceTimeStamp
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
        private double _maximumOccuranceTimeStamp;
        public double MaximumOccuranceTimeStamp
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
        /// Alarm status
        /// </summary>
        private int _alarmStatus;
        public int AlarmStatus
        {
            get => _alarmStatus;
            set => SetProperty(ref _alarmStatus, value);
        }

        /// <summary>
        /// Alarm being processed
        /// </summary>
        private int _alarmBeingProcessed;
        public int AlarmBeingProcessed
        {
            get => _alarmBeingProcessed;
            set => SetProperty(ref _alarmBeingProcessed, value);
        }

        /// <summary>
        /// Alarm delay
        /// </summary>
        private double _alarmDeley;
        public double AlarmDelay
        {
            get => _alarmDeley;
            set => SetProperty(ref _alarmDeley, value);
        }

        /// <summary>
        /// Low alarm limit
        /// </summary>
        private double _lowAlarmLimit;
        public double LowAlarmLimit
        {
            get => _lowAlarmLimit;
            set => SetProperty(ref _lowAlarmLimit, value);
        }

        /// <summary>
        /// Low warning limit
        /// </summary>
        private double _lowWarningLimit;
        public double LowWarningLimit
        {
            get => _lowWarningLimit;
            set => SetProperty(ref _lowWarningLimit, value);
        }

        /// <summary>
        /// High alarm limit
        /// </summary>
        private double _highAlarmLimit;
        public double HighAlarmLimit
        {
            get => _highAlarmLimit;
            set => SetProperty(ref _highAlarmLimit, value);
        }

        /// <summary>
        /// High warning limit
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
        /// Present unix time
        /// </summary>
        private string _currentDateTime;
        public string CurrentDateTime
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }

        #endregion

        #region Parsing Routines

        /// <summary>
        /// Get message counters from remote.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetMessageCounterValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "message counter values"
            if (!StartMessageCounterValueRecord && characteristicValue.Contains("{F"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    MessageCounterValue.Append(characteristicValue);
                    SerializeStringToSensor(MessageCounterValue.ToString(), "F");
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
                    SerializeStringToSensor(MessageCounterValue.ToString(), "F");
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
        private void GetSensorPlotValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor plot values"
            if (!StartSensorBufferedValueRecord && characteristicValue.Contains("{J"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    BufferedSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(BufferedSensorValue.ToString());
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
                    SerializeStringToSensor(BufferedSensorValue.ToString());
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
        private void GetSensorStatisticsValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor statistics values"
            if (!StartSensorStatisticsValueRecord && characteristicValue.Contains("{H"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
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
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
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
        private void GetSensorLimitsValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "sensor limits values"
            if (!StartSensorLimitValueRecord && characteristicValue.Contains("{G"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
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
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
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
        private void SerializeStringToSensor(string sensorValues, string conversionType = "")
        {
            switch (conversionType)
            {
                case "F":
                    //_userDialogs.Alert($"(F) Message Counters Data: {sensorValues}", "CIMScan RemoteManager");

                    // "F" Message counter data serialization
                    TotalOutgoingMessages = sensorValues.Substring(2, 2).SafeHexToInt();
                    TotalOutgoingRetries = sensorValues.Substring(4, 2).SafeHexToInt();
                    TotalOutgoingValues = sensorValues.Substring(6, 2).SafeHexToInt();
                    TotalIncomingMessages = sensorValues.Substring(8, 2).SafeHexToInt();
                    TotalIncomingErrors = sensorValues.Substring(10, 2).SafeHexToInt();
                    LastServerMessageReceived = sensorValues.Substring(12, 2).SafeHexToInt();
                    TotalActiveSensors = sensorValues.Substring(14, 2).SafeHexToInt();
                    TotalRecordsInHistoryBuffer = sensorValues.Substring(16, 2).SafeHexToInt();
                    CurrentDateTime = sensorValues.Substring(20, 8).SafeConvert<string>("");

                    _userDialogs.Alert($"(F) Message Counters TotalActiveSensors: {TotalActiveSensors}", "CIMScan RemoteManager");
                    _userDialogs.Alert($"(F) Message Counters CurrentDateTime: {CurrentDateTime}", "CIMScan RemoteManager");

                    break;
                default:
                    // Split by tab delimiter
                    string[] splitSensorValues = sensorValues.Split('\t');

                    if (SensorCommandType == SensorCommand.Plot)
                    {
                        _userDialogs.Alert($"(J) Buffered Data: {sensorValues}", "CIMScan RemoteManager");

                        // "J" Sensor plot data serialization
                        var sensorPlot = new SensorPlot
                        {
                            Points = splitSensorValues[0].SafeHexToInt(),
                            TimeStamp = splitSensorValues[6].SafeHexToInt(),
                            CurrentValue = splitSensorValues[8].SafeHexToDouble()
                        };
                        // Add sensor to list
                        SensorPlotCollection.Add(sensorPlot);
                    }
                    else if (SensorCommandType == SensorCommand.Statistics)
                    {
                        //_userDialogs.Alert($"(H) Statistics Data: {sensorValues}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(H) SensorIndexSelected: {SensorIndexSelected}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(H) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                        // Only update the values if we have a match
                        if (SensorIndexSelected == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0))
                        {
                            // "H" Sensor data serialization
                            MaximumValue = splitSensorValues[1].SafeHexToDouble();
                            MaximumOccuranceTimeStamp = splitSensorValues[2].SafeHexToInt();
                            MinimumValue = splitSensorValues[3].SafeHexToDouble();
                            MinimumOccuranceTimeStamp = splitSensorValues[4].SafeHexToInt();
                            AverageValue = splitSensorValues[5].SafeHexToDouble();
                            TimeStamp = splitSensorValues[6].SafeHexToInt();
                        }
                    }
                    else if (SensorCommandType == SensorCommand.Limits)
                    {
                        //_userDialogs.Alert($"(G) Limits Data: {sensorValues}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(G) SensorIndexSelected: {SensorIndexSelected}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(G) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('G') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                        // Only update the values if we have a match
                        if (SensorIndexSelected == splitSensorValues[0]
                                .Substring(splitSensorValues[0].LastIndexOf('G') + 1).SafeConvert<int>(0))
                        {
                            // "G" Sensor data serialization
                            AlarmStatus = splitSensorValues[1].SafeHexToInt();
                            AlarmBeingProcessed = splitSensorValues[2].SafeHexToInt();
                            AlarmDelay = splitSensorValues[3].SafeHexToDouble();
                            LowAlarmLimit = splitSensorValues[4].SafeHexToInt();
                            LowWarningLimit = splitSensorValues[5].SafeHexToDouble();
                            HighWarningLimit = splitSensorValues[6].SafeHexToInt();
                            HighAlarmLimit = splitSensorValues[7].SafeHexToDouble();
                        }
                    }
                    break;
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
        private void RxCharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            try
            {
                // Get data based on tab selected
                if (SensorCommandType == SensorCommand.Plot)
                {
                    GetSensorPlotValues(CharacteristicValue);
                }
                else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                {
                    GetSensorStatisticsValues(CharacteristicValue);
                    GetSensorLimitsValues(CharacteristicValue);
                }

                // Get message counter values from remote to determine if
                // we have acquired or lost sensors.  Also grabs time stamp.
                GetMessageCounterValues(CharacteristicValue);

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
                _sensorPlotCollection = new FullyObservableCollection<SensorPlot>();
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
                // Get our Adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid).ConfigureAwait(true);

                // Get write characteristic service
                TxCharacteristic = await _service.GetCharacteristicAsync(TxUuid).ConfigureAwait(true);

                //// Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    // Send a refresh command
                    await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(true);

                    // Now setup plot, statistics, and limits commands
                    //string updateValue = string.Empty;
                    //if (SensorCommandType == SensorCommand.Plot)
                    //{
                    //    updateValue = "{c" + SensorIndex + "}";
                    //}
                    //else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                    //{
                    //    updateValue = "{Y}";
                    //}

                    // Send a refresh command
                    //await TxCharacteristic.WriteAsync(updateValue.StrToByteArray()).ConfigureAwait(true);
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
                _userDialogs.Alert(ex.Message);
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
                SensorIndexSelected = Convert.ToInt32(parameters.Data[SensorIdKey]);

                // Get sensor name from app context
                if (Application.Current.Properties.ContainsKey("CurrentSensorName"))
                {
                    SensorName = Convert.ToString(Application.Current.Properties["CurrentSensorName"]);
                    // Notify property changed
                    RaisePropertyChanged(() => SensorName);
                }

                //_userDialogs.Alert(SensorIndexSelected.ToString(), "Sensor Index Selected");

                // Notify property changed
                RaisePropertyChanged(() => SensorIndexSelected);

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
                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
            }
        }

        /// <summary>
        /// On Resume
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            if (!UpdatesStarted)
            {
                StartUpdates();
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
        /// Start bluetooth characteristics updating.
        /// </summary>
        private async void StartUpdates()
        {
            try
            {
                UpdatesStarted = true;
                
                // Send refresh command to remote
                string updateValue = string.Empty;
                if (SensorCommandType == SensorCommand.Plot)
                {
                    updateValue = "{c" + SensorIndex + "}";
                }
                else if (SensorCommandType == SensorCommand.Statistics || SensorCommandType == SensorCommand.Limits)
                {
                    updateValue = "{Y}";
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
                HockeyApp.MetricsManager.TrackEvent($"(StartUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
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
                _userDialogs.Alert(ex.Message);
            }
        }
        
    }
}