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
using MvvmCross.Plugins.Messenger;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;


namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorPlotViewModel : BaseViewModel
    {
        private readonly MvxSubscriptionToken _subscriptionToken;

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
        /// Sensor index (unique id)
        /// </summary>
        public string SensorIndex { get; set; }

        /// <summary>
        /// Sensor serial number (unique id)
        /// </summary>
        public string SensorSerialNumber { get; set; }

        /// <summary>
        /// Sensor
        /// </summary>
        public Sensor Sensor { get; set; }

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
        public bool StartBufferedSensorValueRecord { get; set; } = false;

        /// <summary>
        ///  "J" = buffered sensor data
        ///   number of points
        ///   time
        ///   value
        /// </summary>
        public readonly StringBuilder BufferedSensorValue = new StringBuilder("");
        

        /// <summary>
        /// Sensor view model constructor
        /// </summary>
        /// <param name="bluetoothLe"></param>
        /// <param name="adapter"></param>
        /// <param name="userDialogs"></param>
        public SensorPlotViewModel(IMvxMessenger messenger, IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            try
            {
                _subscriptionToken = messenger.Subscribe<SensorMessage>(OnSensorMessage);
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

        private void OnSensorMessage(SensorMessage sensorMessage)
        {
            // Set sensor values
            Sensor = sensorMessage.Sensor;

            _userDialogs.Alert($"Sensor OnSensorMessage: {Sensor.SensorIndex.ToString()}", "CIMScan Remote Manager");


            SensorIndex = Sensor.SensorIndex.ToString();
        }

        /// <summary>
        /// On Resume
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            _userDialogs.Alert("SensorPlot :: Resume");
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
                // Get our Adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid).ConfigureAwait(true);

                // Get write characteristic service
                TxCharacteristic = await _service.GetCharacteristicAsync(TxUuid).ConfigureAwait(true);

                // Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    string updateValue = "{c" + SensorIndex + "}";
                    // Send a refresh command
                    await TxCharacteristic.WriteAsync(updateValue.StrToByteArray()).ConfigureAwait(true);
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
                
                //_userDialogs.Alert($"Sensor Index: {parameters.Data[SensorIdKey]}", "CIMScan Remote Manager");

                SensorIndex = parameters.Data[SensorIdKey];

                RaisePropertyChanged(nameof(SensorIndex));

                // Get device from bundle
                _device = GetSensorDeviceBundle(parameters);

                // Set device name
                DeviceName = _device.Name;

                //_userDialogs.Alert($"Device Name: {DeviceName}", "CIMScan Remote Manager");

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
                UpdatesStarted = true;

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                RxCharacteristic.ValueUpdated += RxCharacteristicOnValueUpdated;

                // Send refresh command to remote
                string updateValue = "{c" + SensorIndex + "}";
                await TxCharacteristic.WriteAsync(updateValue.StrToByteArray()).ConfigureAwait(true);
                // Start updates from bluetooth service
                await RxCharacteristic.StartUpdatesAsync().ConfigureAwait(true);

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
                RaisePropertyChanged(() => UpdateButtonText);
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
                // Get buffered sensor data
                GetBufferedSensorValues(CharacteristicValue);

                // Notify property changed
                RaisePropertyChanged(() => CharacteristicValue);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(CharacteristicOnValueUpdated) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Get Buffered values for sensor from characteristic data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetBufferedSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "buffered sensor values"
            if (!StartBufferedSensorValueRecord && characteristicValue.Contains("{J"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    BufferedSensorValue.Append(characteristicValue);
                    SerializeStringToSensor(BufferedSensorValue.ToString());
                    BufferedSensorValue.Clear();
                    StartBufferedSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    BufferedSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartBufferedSensorValueRecord = true;
                }
            }
            else if (StartBufferedSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    BufferedSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    SerializeStringToSensor(BufferedSensorValue.ToString());
                    BufferedSensorValue.Clear();
                    StartBufferedSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    BufferedSensorValue.Append(characteristicValue);
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

        /// <summary>
        /// Sensor selected (navigate to sensor plot page)
        /// </summary>
        public void NavigateToSensorPlotPage(Sensor sensor)
        {
            if (sensor != null)
            {
                // Navigate to sensor plot
                var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { SensorIdKey, sensor.SensorIndex.ToString() } });
                ShowViewModel<SensorPlotViewModel>(bundle);
            }
        }

        /// <summary>
        /// Sensor selected (navigate to sensor statistics page)
        /// </summary>
        public void NavigateToSensorStatisticsPage(Sensor sensor)
        {
            if (sensor != null)
            {
                // Navigate to sensor plot
                var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { SensorIdKey, sensor.SensorIndex.ToString() } });
                ShowViewModel<SensorStatisticsViewModel>(bundle);
            }
        }

    }
}