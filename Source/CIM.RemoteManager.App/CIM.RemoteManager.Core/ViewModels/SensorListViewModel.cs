using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ISensor> Sensors { get; set; } = new ObservableCollection<ISensor>();

        public string UpdateButtonText => _updatesStarted ? "Stop updates" : "Start updates";

        public bool StartFullSensorValueRecord { get; set; } = false;
        public bool StartAverageSensorValueRecord { get; set; } = false;
        public bool StartUnfilteredSensorValueRecord { get; set; } = false;
        public bool StartUnfilteredFloatingPointSensorValueRecord { get; set; } = false;
        

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

        /// <summary>
        ///  "C" = unfiltered (current) value
        ///   # | time | current value 
        /// </summary>
        public readonly StringBuilder UnfilteredSensorValue = new StringBuilder("");

        /// <summary>
        ///  "C" = unfiltered floating point (current) value
        ///   # | time | current value 
        /// </summary>
        public readonly StringBuilder UnfilteredFloatingPointSensorValue = new StringBuilder("");
        

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
        

        private async void InitRemote()
        {
            if (_device == null)
            {
                throw new ArgumentNullException(nameof(_device));
            }

            try
            {
                // Show loading indicator
                _userDialogs.ShowLoading("Loading DA-12 data...");

                // Get our adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid);

                _tx = await _service.GetCharacteristicAsync(TxUuid);

                await Task.Delay(TimeSpan.FromSeconds(1));

                await _tx.WriteAsync("{Y}".StrToByteArray());

                await Task.Delay(TimeSpan.FromSeconds(1));

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
                //Messages.Insert(0, $"Incoming value: {CharacteristicValue}");

                // Get full sensor values
                GetFullSensorValues(CharacteristicValue);
                // Get average sensor values
                GetAverageSensorValues(CharacteristicValue);
                // Get unfiltered (current) sensor values
                GetUnfilteredSensorValues(CharacteristicValue);
                // Get unfiltered floating point (current) sensor values
                GetUnfilteredFloatingPointSensorValues(CharacteristicValue);

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
                    //sMessages.Insert(0, $"Full (A): {FullSensorValue}");
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
                    //Messages.Insert(0, $"Full (A): {FullSensorValue}");
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
        /// Get Average values for sensor from buffered data
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
                    //Messages.Insert(0, $"Average (B): {AverageSensorValue}");
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
                    //Messages.Insert(0, $"Average (B): {AverageSensorValue}");
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
        /// Get unfiltered (current) values for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetUnfilteredSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "unfiltered (current) sensor values"
            if (!StartUnfilteredSensorValueRecord && characteristicValue.Contains("{C"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredSensorValue.Append(characteristicValue.Replace("{", "").GetUntilOrEmpty());
                    //Messages.Insert(0, $"Unfiltered (C): {UnfilteredSensorValue}");
                    SerializeStringToSensor(UnfilteredSensorValue.ToString(), "C");
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
            else if (StartFullSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    //Messages.Insert(0, $"Unfiltered (C): {UnfilteredSensorValue}");
                    SerializeStringToSensor(UnfilteredSensorValue.ToString(), "C");
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
        /// Get unfiltered (current) values from floating point for sensor from buffered data
        /// </summary>
        /// <param name="characteristicValue"></param>
        private void GetUnfilteredFloatingPointSensorValues(string characteristicValue)
        {
            if (String.IsNullOrEmpty(characteristicValue)) return;

            // Start reading all "unfiltered (current) sensor values"
            if (!StartUnfilteredFloatingPointSensorValueRecord && characteristicValue.Contains("{I"))
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredFloatingPointSensorValue.Append(characteristicValue.Replace("{", "").GetUntilOrEmpty());
                    //Messages.Insert(0, $"Unfiltered (I): {UnfilteredFloatingPointSensorValue}");
                    SerializeStringToSensor(UnfilteredFloatingPointSensorValue.ToString(), "I");
                    UnfilteredFloatingPointSensorValue.Clear();
                    StartUnfilteredFloatingPointSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    UnfilteredFloatingPointSensorValue.Append(characteristicValue.Trim(new Char[] { '{' }));
                    StartUnfilteredFloatingPointSensorValueRecord = true;
                }
            }
            else if (StartFullSensorValueRecord)
            {
                // If we hit an end char } then record all data up to it
                if (characteristicValue.Contains("}"))
                {
                    UnfilteredFloatingPointSensorValue.Append(characteristicValue.GetUntilOrEmpty());
                    //Messages.Insert(0, $"Unfiltered (I): {UnfilteredFloatingPointSensorValue}");
                    SerializeStringToSensor(UnfilteredFloatingPointSensorValue.ToString(), "I");
                    UnfilteredFloatingPointSensorValue.Clear();
                    StartUnfilteredFloatingPointSensorValueRecord = false;
                }
                else
                {
                    // Read all characters in buffer while we are within the {}
                    UnfilteredFloatingPointSensorValue.Append(characteristicValue);
                }
            }
        }

        /// <summary>
        /// Serialize tab based sensor data to stronly typed Sensor
        /// </summary>
        /// <param name="sensorValues"></param>
        /// <param name="conversionType"></param>
        private void SerializeStringToSensor(string sensorValues, string conversionType)
        {
            // Split by tab delimeter
            string[] splitSensorValues = sensorValues.Split('\t');
            
            switch (conversionType)
            {
                case "A":
                    // "A" Sensor data serialization
                    var sensor = new Sensor
                    {
                        Index = splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0),
                        SerialNumber = splitSensorValues[1],
                        Name = splitSensorValues[2],
                        SensorType = splitSensorValues[3],
                        Scale = splitSensorValues[4].SafeHexToFloat(),
                        Offset = splitSensorValues[5].SafeHexToFloat(),
                        TimeStamp = splitSensorValues[6].SafeHexToInt(),
                        AverageValue = splitSensorValues[7].SafeHexToFloat(),
                        CurrentValue = splitSensorValues[8].SafeHexToFloat(),
                        DecimalLocation = splitSensorValues[9].SafeHexToInt(),
                        StatisticsTotalCalcSettings = splitSensorValues[10].SafeConvert<string>("")
                    };
                    // Add sensor to list
                    Sensors.Add(sensor);
                    break;
                case "B":
                    // "B" Sensor data serialization
                    // Update Sensor list by index
                    foreach (var sensorValue in Sensors.Where(s => s.Index == splitSensorValues[0].SafeConvert<int>(0)))
                    {
                        sensorValue.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorValue.AverageValue = splitSensorValues[1].SafeHexToFloat();
                        sensorValue.CurrentValue = splitSensorValues[2].SafeHexToFloat();
                        sensorValue.DecimalLocation = splitSensorValues[3].SafeHexToInt();
                        sensorValue.StatisticsTotalCalcSettings = splitSensorValues[4];
                    }
                    break;
                case "C":
                    // "C" Sensor data serialization
                    // Update Sensor list by index
                    foreach (var sensorValue in Sensors.Where(s => s.Index == splitSensorValues[0].SafeConvert<int>(0)))
                    {
                        sensorValue.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorValue.CurrentValue = splitSensorValues[1].SafeHexToFloat();
                    }
                    break;
                case "I":
                    // "I" Sensor data serialization
                    // Update Sensor list by index
                    foreach (var sensorValue in Sensors.Where(s => s.Index == splitSensorValues[0].SafeConvert<int>(0)))
                    {
                        sensorValue.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorValue.CurrentValue = splitSensorValues[1].SafeHexToFloat();
                    }
                    break;
                default:
                    throw new Exception($"nameof(conversionType) not defined");
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