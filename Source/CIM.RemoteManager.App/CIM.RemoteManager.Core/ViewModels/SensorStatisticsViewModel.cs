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


namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorStatisticsViewModel : BaseViewModel
    {
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
        /// Sensor sttistics
        /// </summary>
        SensorStatistics _sensorStatistics;
        public SensorStatistics SensorStatistics
        {
            get => _sensorStatistics;
            set => SetProperty(ref _sensorStatistics, value);
        }

        /// <summary>
        /// Show sensor updates mode
        /// </summary>
        public string UpdateButtonText => UpdatesStarted ? "Updates On" : "Updates Off";

        /// <summary>
        /// Sensor record types (serialized into models later)
        /// </summary>
        public bool StartStatisticsSensorValueRecord { get; set; } = false;

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
        /// Sensor statistics view model constructor
        /// </summary>
        /// <param name="bluetoothLe">Bluetooth LE obj</param>
        /// <param name="adapter">Bluetooth LE adapter</param>
        /// <param name="userDialogs">User dialogs</param>
        public SensorStatisticsViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            try
            {
                _bluetoothLe = bluetoothLe;
                _userDialogs = userDialogs;
                // Events
                _bluetoothLe.StateChanged += OnStateChanged;

                _userDialogs.Alert("SensorStats :: SensorStatisticsViewModel");

                // Register event for device connection lost
                Adapter.DeviceConnectionLost += OnDeviceConnectionLost;

                // New instance of sensor statistics
                _sensorStatistics = new SensorStatistics();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
                Mvx.Trace(ex.Message);
            }
        }
        
        /// <summary>
        /// On Resume
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            _userDialogs.Alert("SensorStats :: Resume");
            // Init from bundle which grabs our device and kicks things off
            InitFromBundle(Bundle);
        }
        
        /// <summary>
        /// Event to handle Bluetooth LE state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            RaisePropertyChanged(nameof(IsStateOn));
            RaisePropertyChanged(nameof(StateText));
        }

        /// <summary>
        /// Event to handle Bluetooth connection changes
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
        /// Setup all the possible Bluetooth LE states
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

                // Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    // Send a refresh command
                    await TxCharacteristic.WriteAsync("{X}".StrToByteArray()).ConfigureAwait(true);
                }
                else
                {
                    _userDialogs.Alert("Cannot write characteristic data to remote!", "CIMScan Remote Manager");
                }

                // Wait 500 milliseconds
                await Task.Delay(500).ConfigureAwait(true);

                // Get Characteristics service
                RxCharacteristic = await _service.GetCharacteristicAsync(RxUuid).ConfigureAwait(true);

                // Wait 500 milliseconds
                await Task.Delay(4500).ConfigureAwait(true);

                // Start updates
                ToggleUpdatesCommand.Execute(null);

                // Hide loading...
                //_userDialogs.HideLoading();

            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                HockeyApp.MetricsManager.TrackEvent($"(InitRemote) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
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
                HockeyApp.MetricsManager.TrackEvent($"(InitFromBundle) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
            }
        }
        
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

        /// <summary>
        /// Start bluetooth characteristics updating
        /// </summary>
        private async void StartUpdates()
        {
            try
            {
                _userDialogs.Alert("Starting updates...", "CIMScan Remote Manager");

                UpdatesStarted = true;

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                RxCharacteristic.ValueUpdated += RxCharacteristicOnValueUpdated;

                // Send refresh command to remote
                await TxCharacteristic.WriteAsync("{X}".StrToByteArray()).ConfigureAwait(true);
                // Start updates from bluetooth service
                await RxCharacteristic.StartUpdatesAsync().ConfigureAwait(true);

                // Let UI know mode we are in
                //RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(StartUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
            }
        }

        /// <summary>
        /// Stop bluetooth characteristics updating
        /// </summary>
        private async void StopUpdates()
        {
            try
            {
                UpdatesStarted = false;

                // Stop updates from bluetooth service
                await RxCharacteristic.StopUpdatesAsync().ConfigureAwait(true);

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                
                // Let UI know mode we are in
                //RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(StopUpdates) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
                _userDialogs.Alert(ex.Message);
            }
        }

        /// <summary>
        /// Rx characteristic updated event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="characteristicUpdatedEventArgs"></param>
        private void RxCharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            try
            {
                _userDialogs.Alert($"CharacteristicValue: {CharacteristicValue}", "CIMScan Remote Manager");

                // Get statistics sensor values
                GetStatisticsSensorValues(CharacteristicValue);

                // Notify property changed
                RaisePropertyChanged(() => CharacteristicValue);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(CharacteristicOnValueUpdated) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
            
        }

        /// <summary>
        /// Get Statistics values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetStatisticsSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "full sensor values"
            if (!StartStatisticsSensorValueRecord && characteristicValue.Contains("{H"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartStatisticsSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartStatisticsSensorValueRecord = true;
                }
            }
            else if (StartStatisticsSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    StatisticsSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    SerializeStringToSensor(StatisticsSensorValue.ToString());
                    StatisticsSensorValue.Clear();
                    StartStatisticsSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    StatisticsSensorValue.Append(characteristicValue);
                }
            }
        }
        
        /// <summary>
        /// Serialize tab based sensor data to strongly typed Sensor model
        /// </summary>
        /// <param name="sensorValues"></param>
        private void SerializeStringToSensor(string sensorValues)
        {
            // Split by tab delimiter
            string[] splitSensorValues = sensorValues.Split('\t');

            _userDialogs.Alert($"(H) Sensor Values: {sensorValues}", "CIMScan RemoteManager");

            // "H" Sensor data serialization
            SensorStatistics.SensorIndex = splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('H') + 1).SafeConvert<int>(0);
            SensorStatistics.MaximumValue = splitSensorValues[1].SafeHexToDouble();
            SensorStatistics.MaximumOccuranceTimeStamp = splitSensorValues[2].SafeHexToInt();
            SensorStatistics.MinimumValue = splitSensorValues[3].SafeHexToDouble();
            SensorStatistics.MinimumOccuranceTimeStamp = splitSensorValues[4].SafeHexToInt();
            SensorStatistics.AverageValue = splitSensorValues[5].SafeHexToDouble();
            SensorStatistics.TimeStamp = splitSensorValues[6].SafeHexToInt();
            // Notify property changed to update UI
            RaisePropertyChanged(()=> SensorStatistics);
        }
        
    }
}