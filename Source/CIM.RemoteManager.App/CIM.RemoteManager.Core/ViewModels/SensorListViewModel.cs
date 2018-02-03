using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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


namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorListViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private IDevice _device;
        private IService _service;

        private ICharacteristic _tx;
        private ICharacteristic _rx;

        // UUIDs for UART service and associated characteristics.
        public static Guid UartUuid = Guid.Parse("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
        public static Guid TxUuid = Guid.Parse("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        public static Guid RxUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        // UUID for the UART BTLE client characteristic which is necessary for notifications.
        public static Guid ClientUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        // UUIDs for the Device Information service and associated characeristics.
        public static Guid DisUuid = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
        public static Guid DisManufUuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        public static Guid DisModelUuid = Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb");
        public static Guid DisHwrevUuid = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");
        public static Guid DisSwrevUuid = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");

        private bool _updatesStarted;
        public ICharacteristic Characteristic { get; private set; }

        public string CharacteristicValue => Characteristic?.Value.BytesToStringConverted();

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public string UpdateButtonText => _updatesStarted ? "Stop updates" : "Start updates";

        private IList<ISensor> _sensors;

        public IList<ISensor> Sensors
        {
            get => _sensors;
            private set => SetProperty(ref _sensors, value);
        }

        public bool StartFullSensorValueRecord { get; set; } = false;
        public bool StartAverageSensorValueRecord { get; set; } = false;
        public bool StartUnfilteredSensorValueRecord { get; set; } = false;

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
        StringBuilder FullSensorValue = new StringBuilder("");

        /// <summary>
        ///  "B" = average value for the sensor identified by #
        ///   # | time | average value | alarm status
        /// </summary>
        StringBuilder AverageSensorValue = new StringBuilder("");

        /// <summary>
        ///  "C" = unfiltered (current) value
        ///   # | time | current value 
        /// </summary>
        StringBuilder UnfilteredSensorValue = new StringBuilder("");

        public SensorListViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;
            
            // Send a refresh command to our remote to start pulling all our data
            //InitRemote();
        }

        public override void Resume()
        {
            base.Resume();

            //LoadSensorData();
        }

        private async void LoadSensorData()
        {
            try
            {
                _userDialogs.ShowLoading("Loading sensor data...");

                Guid deviceGuid = _device.Id;
                
                //Sensors = await GetSensorsAsync();
                
                //RaisePropertyChanged(() => Sensors);

                _userDialogs.HideLoading();

            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Error while loading sensor data");
                Mvx.Trace(ex.Message);
            }
            finally
            {
                _userDialogs.HideLoading();
            }
        }

        private Task<IList<ISensor>> GetSensorsAsync()
        {


            return null;

        }



        //public MvxCommand WriteCommand => new MvxCommand(WriteValueAsync);

        private async void InitRemote()
        {
            if (_device == null)
            {
                throw new ArgumentNullException(nameof(_device));
            }

            //if (string.IsNullOrEmpty(characteristicValue))
            //{
            //    throw new ArgumentNullException(nameof(characteristicValue));
            //}


            try
            {
                //var data = GetBytes(result.Text);

                // Show loading indicator
                _userDialogs.ShowLoading("Loading DA-12 data...");

                // Get our adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid);

                Characteristic = await _service.GetCharacteristicAsync(RxUuid);


                
                //var service = await _device.GetServiceAsync(UartUuid);

                // Get our adafruit bluetooth characteristic
                // Tx (Write)
                // _tx = await service.GetCharacteristicAsync(TxUuid);

                // Write values async
                //await _tx.WriteAsync("{Y}".StrToByteArray());


                // Get our adafruit bluetooth characteristic
                // RX (read)
                //ICharacteristic _rx = await service.GetCharacteristicAsync(RxUuid);

                // Start updates
                //StartUpdates();

                // Hide loading...
                _userDialogs.HideLoading();

                //RaisePropertyChanged(() => CharacteristicValue);

               // _userDialogs.Toast($"Wrote value {characteristicValue}");

            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError(ex.Message);
            }

        }

        private static byte[] GetBytes(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
        }
        
        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);
            
            _device = GetDeviceFromBundle(parameters);

            InitRemote();

            if (_device == null)
            {
                Close(this);
            }
        }

        public MvxCommand ToggleUpdatesCommand => new MvxCommand((() =>
        {
            if (_updatesStarted)
            {
                StopUpdates();
            }
            else
            {
                StartUpdates();
            }
        }));

        public MvxCommand ReadCommand => new MvxCommand(ReadValueAsync);

        private async void ReadValueAsync()
        {
            if (Characteristic == null)
                return;

            try
            {
                _userDialogs.ShowLoading("Reading characteristic value...");

                await Characteristic.ReadAsync();

                RaisePropertyChanged(() => CharacteristicValue);

                Messages.Insert(0, $"Read value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError(ex.Message);

                Messages.Insert(0, $"Error {ex.Message}");

            }
            finally
            {
                _userDialogs.HideLoading();
            }

        }

        public MvxCommand WriteCommand => new MvxCommand(WriteValueAsync);

        private async void WriteValueAsync()
        {
            try
            {
                var result =
                    await
                        _userDialogs.PromptAsync("Input a value (as hex whitespace separated)", "Write value",
                            placeholder: CharacteristicValue);

                if (!result.Ok)
                    return;

                var data = GetBytes(result.Text);

                _userDialogs.ShowLoading("Write characteristic value");
                await Characteristic.WriteAsync(data);
                _userDialogs.HideLoading();

                RaisePropertyChanged(() => CharacteristicValue);
                Messages.Insert(0, $"Wrote value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError(ex.Message);
            }

        }

        private async void StartUpdates()
        {
            try
            {
                _updatesStarted = true;

                Characteristic.ValueUpdated -= CharacteristicOnValueUpdated;
                Characteristic.ValueUpdated += CharacteristicOnValueUpdated;
                await Characteristic.StartUpdatesAsync();

                Messages.Insert(0, $"Start updates");

                RaisePropertyChanged(() => UpdateButtonText);
            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message);
            }
        }

        private async void StopUpdates()
        {
            try
            {
                _updatesStarted = false;

                await Characteristic.StopUpdatesAsync();
                Characteristic.ValueUpdated -= CharacteristicOnValueUpdated;

                Messages.Insert(0, $"Stop updates");

                RaisePropertyChanged(() => UpdateButtonText);

            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message);
            }
        }

        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            try
            {
                Messages.Insert(0, $"Updated value: {CharacteristicValue}");

                // Get full sensor values
                GetFullensorValues(CharacteristicValue);
                // Get average sensor values
                GetAverageSensorValues(CharacteristicValue);
                // Get unfiltered (current) sensor values
                //GetUnfilteredSensorValues(CharacteristicValue);

                //Messages.Insert(0, $"Updated value: {CharacteristicValue}");

                RaisePropertyChanged(() => CharacteristicValue);
            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message);
            }
            
        }

        /// <summary>
        /// Get Full values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetFullensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "average sensor values"
            if (characteristicValue.Contains("{A"))
            {
                StartFullSensorValueRecord = true;
            }
            // If we hit an end char } then record all data up to it
            if (characteristicValue.Contains("}"))
            {
                FullSensorValue.Append(characteristicValue.GetUntilOrEmpty("}"));
                StartFullSensorValueRecord = false;
                Messages.Insert(0, $"Full Sensor Value: {FullSensorValue}");
            }
            // Read all characters in buffer while we are within the {}
            if (StartFullSensorValueRecord)
            {
                FullSensorValue.Append(characteristicValue);
            }
        }

        /// <summary>
        /// Get Average values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetAverageSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "average sensor values"
            if (characteristicValue.Contains("{B"))
            {
                StartAverageSensorValueRecord = true;
            }
            // If we hit an end char } then record all data up to it
            if (characteristicValue.Contains("}"))
            {
                AverageSensorValue.Append(characteristicValue.GetUntilOrEmpty("}"));
                StartAverageSensorValueRecord = false;
                Messages.Insert(0, $"Average Sensor Value: {AverageSensorValue}");
            }
            // Read all characters in buffer while we are within the {}
            if (StartAverageSensorValueRecord)
            {
                AverageSensorValue.Append(characteristicValue);
            }
        }

        /// <summary>
        /// Get unfiltered (current) values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetUnfilteredSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "average sensor values"
            if (characteristicValue.ToLower().Contains("{C"))
            {
                StartUnfilteredSensorValueRecord = true;
            }
            // If we hit an end char } then record all data up to it
            if (characteristicValue.Contains("}"))
            {
                UnfilteredSensorValue.Append(characteristicValue.GetUntilOrEmpty("}"));
                StartUnfilteredSensorValueRecord = false;
                Messages.Insert(0, $"Current Sensor Value: {UnfilteredSensorValue}");
            }
            // Read all characters in buffer while we are within the {}
            if (StartUnfilteredSensorValueRecord)
            {
                UnfilteredSensorValue.Append(characteristicValue);
            }
        }


        public IService SelectedSensor
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { SensorIdKey, value.Id.ToString() } });

                    ShowViewModel<SensorPlotViewModel>(bundle);
                }

                RaisePropertyChanged();

            }
        }

        
    }
}