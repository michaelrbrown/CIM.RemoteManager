using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class SensorListViewModel : BaseViewModel
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
        /// Sensor collection
        /// </summary>
        FullyObservableCollection<Sensor> _sensorCollection;
        public FullyObservableCollection<Sensor> SensorCollection
        {
            get => _sensorCollection;
            set => SetProperty(ref _sensorCollection, value);
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
        /// Message counter (F record)
        /// </summary>
        public bool StartMessageCounterValueRecord { get; set; } = false;
        /// <summary>
        /// Sensor record types (A record)
        /// </summary>
        public bool StartFullSensorValueRecord { get; set; } = false;
        public bool StartAverageSensorValueRecord { get; set; } = false;

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
        ///   current datetime
        /// </summary>
        public readonly StringBuilder MessageCounterValue = new StringBuilder("");
        /// <summary>
        ///  "A" = full information
        ///   index (#)
        ///   serial number
        ///   name
        ///   sensor type
        ///   scale
        ///   offset
        ///   timestamp
        ///   average value
        ///   current value
        ///   display conversion code
        ///   decimal location
        ///   statistics total calculation settings
        /// </summary>
        public readonly StringBuilder FullSensorValue = new StringBuilder("");

        /// <summary>
        ///  "B" = average value for the sensor identified by #
        ///   # | time | average value | alarm status
        /// </summary>
        public readonly StringBuilder AverageSensorValue = new StringBuilder("");

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
        /// Get Full values for sensor from buffered data.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetFullSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "full sensor values"
            if (!StartFullSensorValueRecord && characteristicValue.Contains("{A"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    FullSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(FullSensorValue.ToString(), "A");
                    FullSensorValue.Clear();
                    StartFullSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    FullSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartFullSensorValueRecord = true;
                }
            }
            else if (StartFullSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    FullSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    SerializeStringToSensor(FullSensorValue.ToString(), "A");
                    FullSensorValue.Clear();
                    StartFullSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    FullSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Get Average values for sensor from buffered data.
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetAverageSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "average sensor values"
            if (!StartAverageSensorValueRecord && characteristicValue.Contains("{B"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    AverageSensorValue.Append(characteristicValue.Replace("{", "").GetUntilOrEmpty());
                    SerializeStringToSensor(AverageSensorValue.ToString(), "B");
                    AverageSensorValue.Clear();
                    StartAverageSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    AverageSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartAverageSensorValueRecord = true;
                }
            }
            else if (StartAverageSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    AverageSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    SerializeStringToSensor(AverageSensorValue.ToString(), "B");
                    AverageSensorValue.Clear();
                    StartAverageSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    AverageSensorValue.Append(characteristicValue.Replace("{", ""));
                }
            }
        }

        /// <summary>
        /// Checks if remote date year is before 2009 which means the time has not been set yet.
        /// Sets the time to Unix time if this is true.
        /// </summary>
        /// <param name="txCharacteristic">The TX characteristic.</param>
        /// <param name="remoteUnixDateTime">The remote Unix date time.</param>
        public async void HandleRemoteDateTimeValidation(ICharacteristic txCharacteristic, double remoteUnixDateTime)
        {
            Device.BeginInvokeOnMainThread(() => {
                _userDialogs.Alert($"(F) Message Counters TimeStamp: {remoteUnixDateTime.UnixTimeStampToDateTime().Year}", "CIMScan RemoteManager");
            });

            // Validate our station Unix time converted to windows time is less
            // than 2009.  If it is we know the station time needs to be set.
            if (remoteUnixDateTime.UnixTimeStampToDateTime().Year < 2009)
            {
                // Get Unix timestamp "now" as UTC
                Int32 unixTimestampUtc = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Format DA-12 station Unix timestamp
                // T - is to set the station time,
                // 0 - is the data type
                // 00 - device index / unused
                // Next set of digits is Unix time UTC
                string remoteUnitTimestamp = "{T000" + unixTimestampUtc + "}";

                // Send set Unix UTC time command to remote
                await txCharacteristic.WriteAsync(remoteUnitTimestamp.StrToByteArray()).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Serialize tab based sensor data to strongly typed Sensor model.
        /// </summary>
        /// <param name="sensorValues"></param>
        /// <param name="conversionType"></param>
        private void SerializeStringToSensor(string sensorValues, string conversionType)
        {
            // Split by tab delimiter
            string[] splitSensorValues = sensorValues.Split('\t');

            // What type of record are we parsing / serializing?
            switch (conversionType)
            {
                case "F":
                    //_userDialogs.Alert($"(F) Message Counters Data: {sensorValues}", "CIMScan RemoteManager");

                    // "F" Message counter data serialization
                    TotalOutgoingMessages = sensorValues.Substring(1, 2).SafeHexToInt();
                    TotalOutgoingRetries = sensorValues.Substring(3, 2).SafeHexToInt();
                    TotalOutgoingValues = sensorValues.Substring(5, 2).SafeHexToInt();
                    TotalIncomingMessages = sensorValues.Substring(7, 2).SafeHexToInt();
                    TotalIncomingErrors = sensorValues.Substring(9, 2).SafeHexToInt();
                    LastServerMessageReceived = sensorValues.Substring(11, 2).SafeHexToInt();
                    TotalActiveSensors = sensorValues.Substring(13, 2).SafeHexToInt();
                    TotalRecordsInHistoryBuffer = sensorValues.Substring(15, 2).SafeHexToInt();
                    CurrentDateTime = sensorValues.Substring(19, 8).SafeConvert<int>(0);

                    //_userDialogs.Alert($"(F) TotalActiveSensors: {TotalActiveSensors}", "CIMScan RemoteManager");
                    //_userDialogs.Alert($"(F) CurrentDateTime: {CurrentDateTime}", "CIMScan RemoteManager");

                    // New instance of station helper
                    var stationHelper = new StationHelper();
                    // Validate our current remote Unix date time. Update to current Unix UTC date time
                    // if year < 2009.
                    stationHelper.HandleRemoteDateTimeValidation(TxCharacteristic, CurrentDateTime);
                    break;
                case "A":
                    // "A" Sensor data serialization
                    var sensorListItemA = SensorCollection.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0));
                    if (sensorListItemA != null)
                    {
                        //_userDialogs.Alert($"(A) Sensor Index: { splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");

                        // Update sensor items in list
                        //sensorListItemA.SensorIndex = splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0);
                        sensorListItemA.SerialNumber = splitSensorValues[1];
                        sensorListItemA.Name = splitSensorValues[2];
                        sensorListItemA.SensorType = splitSensorValues[3];
                        sensorListItemA.Scale = splitSensorValues[4].SafeConvert<double>(0);
                        sensorListItemA.Offset = splitSensorValues[5].SafeConvert<double>(0);
                        sensorListItemA.TimeStamp = splitSensorValues[6].SafeHexToInt();
                        sensorListItemA.AverageValue = splitSensorValues[7].SafeHexToDouble();
                        sensorListItemA.CurrentValue = splitSensorValues[8].SafeHexToDouble();
                        sensorListItemA.DecimalLocation = splitSensorValues[9].SafeConvert<int>(0);
                        sensorListItemA.StatisticsTotalCalcSettings = splitSensorValues[10];
                        // Notify property changed to update UI
                        RaisePropertyChanged(() => SensorCollection);
                    }
                    else
                    {
                        //_userDialogs.Alert($"(A) (NEW REC) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0)}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(A) Serial Number: {splitSensorValues[1]}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(A) Average Value: {splitSensorValues[7].SafeHexToDouble().ToString()}", "CIMScan RemoteManager");

                        // Create new sensor record for list
                        var sensor = new Sensor
                        {
                            SensorIndex = splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0),
                            SerialNumber = splitSensorValues[1],
                            Name = splitSensorValues[2],
                            SensorType = splitSensorValues[3],
                            Scale = splitSensorValues[4].SafeConvert<double>(0),
                            Offset = splitSensorValues[5].SafeConvert<double>(0),
                            TimeStamp = splitSensorValues[6].SafeHexToInt(),
                            AverageValue = splitSensorValues[7].SafeHexToDouble(),
                            CurrentValue = splitSensorValues[8].SafeHexToDouble(),
                            DecimalLocation = splitSensorValues[9].SafeConvert<int>(0),
                            StatisticsTotalCalcSettings = splitSensorValues[10]
                        };
                        // Add sensor to list
                        SensorCollection.Add(sensor);
                    }
                    break;
                case "B":
                    // "B" Sensor data serialization
                    // Update Sensor list by index
                    var sensorListItemB = SensorCollection.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('B') + 1).SafeConvert<int>(0));
                    if (sensorListItemB != null)
                    {
                        //_userDialogs.Alert($"(B) Sensor Index: {splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('B') + 1).SafeConvert<int>(0).ToString()}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(B) Sensor Index: {splitSensorValues[0]}", "CIMScan RemoteManager");
                        //_userDialogs.Alert($"(B) Average Value: {splitSensorValues[2].SafeHexToDouble().ToString()}", "CIMScan RemoteManager");

                        //sensorListItemB.SensorIndex = splitSensorValues[0].SafeHexToInt();
                        sensorListItemB.TimeStamp = splitSensorValues[1].SafeHexToInt();
                        sensorListItemB.AverageValue = splitSensorValues[2].SafeHexToDouble();
                        sensorListItemB.AlarmStatus = splitSensorValues[3].SafeHexToInt();
                    }
                    // Notify property changed to update UI
                    RaisePropertyChanged(() => SensorCollection);
                    break;
                default:
                    throw new Exception($"nameof(conversionType) not defined");
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
                // Get message counter values from remote to determine if
                // we have acquired or lost sensors.  Also grabs time stamp.
                GetMessageCounterValues(CharacteristicValue);
                // Get full sensor values
                GetFullSensorValues(CharacteristicValue);
                // Get average sensor values
                GetAverageSensorValues(CharacteristicValue);

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
        /// Toggle sensor updates
        /// </summary>
        public MvxCommand ToggleUpdatesCommand => new MvxCommand((() =>
        {
            if (UpdatesStarted)
            {
                StopUpdates();
            }
            else
            {
                StartUpdates();
            }
        }));

        #endregion

        /// <summary>
        /// Sensor view model constructor.
        /// </summary>
        /// <param name="bluetoothLe">Bluetooth LE obj</param>
        /// <param name="adapter">Bluetooth LE adapter</param>
        /// <param name="userDialogs">User dialogs</param>
        public SensorListViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            try
            {
                // Loading indicator
                IsLoading = true;

                _bluetoothLe = bluetoothLe;
                _userDialogs = userDialogs;

                // Events
                _bluetoothLe.StateChanged += OnStateChanged;

                // Register event for device connection lost
                Adapter.DeviceConnectionLost += OnDeviceConnectionLost;

                // Sensor data
                _sensorCollection = new FullyObservableCollection<Sensor>();
            }
            catch (Exception ex)
            {
                // Loading indicator (make sure it turns off even with exception thrown)
                IsLoading = false;

                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
                Mvx.Trace(ex.Message);
            }
        }

        /// <summary>
        /// Initialization of Bluetooth service characteristics.
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
                // Get our AdaFruit Bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid).ConfigureAwait(true);

                // Get write characteristic service
                TxCharacteristic = await _service.GetCharacteristicAsync(TxUuid).ConfigureAwait(true);

                // Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    // Send a refresh command
                    await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(false);
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
                // Loading indicator (make sure it turns off even with exception thrown)
                IsLoading = false;

                _userDialogs.HideLoading();
                HockeyApp.MetricsManager.TrackEvent($"(InitRemote) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
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

                // Get device from bundle
                _device = GetDeviceFromBundle(parameters);

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
                // Loading indicator (make sure it turns off even with exception thrown)
                IsLoading = false;

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

                    //_userDialogs.Alert($"IsLoading: {IsLoading.ToString()}; Updates started: {UpdatesStarted.ToString()}; RxTryCount: {RxTryCount.ToString()}");

                    // Make sure we are done with our initialization before starting updates
                    while (IsLoading && !UpdatesStarted && RxTryCount < 100)
                    {
                        _userDialogs.Alert($"Inside loop - RxTryCount: {RxTryCount.ToString()}");

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
                    //_userDialogs.Alert("HandleUpdatesStarted...");
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
                _userDialogs.Alert(ex.Message);
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

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                RxCharacteristic.ValueUpdated += RxCharacteristicOnValueUpdated;

                // Send refresh command to remote
                await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(true);
                // Start updates from Bluetooth service
                await RxCharacteristic.StartUpdatesAsync().ConfigureAwait(true);

                // Let UI know mode we are in
                RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                HockeyApp.MetricsManager.TrackEvent($"(HandleUpdatesStarted) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
            }
        }

        /// <summary>
        /// Stop Bluetooth characteristics updating.
        /// </summary>
        private async void StopUpdates()
        {
            try
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                // Stop updates from Bluetooth service
                await RxCharacteristic.StopUpdatesAsync().ConfigureAwait(true);

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;

                // Let UI know mode we are in
                RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                UpdatesStarted = false;
                // Notify property changed
                RaisePropertyChanged(() => UpdatesStarted);

                HockeyApp.MetricsManager.TrackEvent($"(StopUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
            }
        }

        /// <summary>
        /// Sensor selected (navigate to sensor plot page).
        /// </summary>
        public void NavigateToSensorDetailsPage(Sensor sensor)
        {
            if (sensor != null)
            {
                // Clear old sensor name
                if (Application.Current.Properties.ContainsKey("CurrentSensorName"))
                {
                    Application.Current.Properties.Remove("CurrentSensorName");
                }
                // Set sensor name for details page
                Application.Current.Properties["CurrentSensorName"] = sensor.Name;

                // Navigate to sensor plot
                var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { SensorIdKey, sensor.SensorIndex.ToString() } });
                ShowViewModel<SensorDetailsViewModel>(bundle);
            }
        }

    }
}