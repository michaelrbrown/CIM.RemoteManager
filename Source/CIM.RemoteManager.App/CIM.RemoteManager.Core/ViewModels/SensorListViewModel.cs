﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private readonly IUserDialogs _userDialogs;
        private IDevice _device;
        private IService _service;

        public ICharacteristic TxCharacteristic { get; private set; }
        public ICharacteristic RxCharacteristic { get; private set; }

        // UUIDs for UART service and associated characteristics.
        public static Guid UartUuid = Guid.Parse("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
        public static Guid TxUuid = Guid.Parse("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        public static Guid RxUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        // UUID for the UART BTLE client characteristic which is necessary for notifications.
        public static Guid ClientUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        // UUIDs for the Device Information service and associated characteristics.
        public static Guid DisUuid = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
        public static Guid DisManufUuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        public static Guid DisModelUuid = Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb");
        public static Guid DisHwrevUuid = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");
        public static Guid DisSwrevUuid = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");

        public bool UpdatesStarted;
        public ICharacteristic Characteristic { get; private set; }

        public string CharacteristicValue => RxCharacteristic?.Value.BytesToStringConverted();

        public string DeviceName { get; set; }

        /// <summary>
        /// Sensor collection
        /// </summary>
        FullyObservableCollection<Sensor> _sensors;
        public FullyObservableCollection<Sensor> Sensors
        {
            get => _sensors;
            set
            {
                _sensors = value;
                RaisePropertyChanged(() => _sensors);
            }
        }

        /// <summary>
        /// Show sensor updates mode
        /// </summary>
        public string UpdateButtonText => UpdatesStarted ? "Updates On" : "Updates Off";

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
        
        /// <summary>
        /// Sensor view model constructor
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="userDialogs"></param>
        public SensorListViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            try
            {
                _userDialogs = userDialogs;
                _sensors = new FullyObservableCollection<Sensor>();
                
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
                
                // Get our Adafruit bluetooth service (UART)
                _service = await _device.GetServiceAsync(UartUuid).ConfigureAwait(true);
                
                // Get write characteristic service
                TxCharacteristic = await _service.GetCharacteristicAsync(TxUuid).ConfigureAwait(true);
                
                // Make sure we can write characteristic data to remote
                if (TxCharacteristic.CanWrite)
                {
                    // Send a refresh command
                    await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(true);
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
                //await Task.Delay(3500).ConfigureAwait(true);

                // Start updates
                //ToggleUpdatesCommand.Execute(null);
                
                // Hide loading...
                _userDialogs.HideLoading();

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
                UpdatesStarted = true;

                // Subscribe to value updated events
                RxCharacteristic.ValueUpdated -= RxCharacteristicOnValueUpdated;
                RxCharacteristic.ValueUpdated += RxCharacteristicOnValueUpdated;

                // Send refresh command to remote
                await TxCharacteristic.WriteAsync("{Y}".StrToByteArray()).ConfigureAwait(true);
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
                // Get full sensor values
                GetFullSensorValues(CharacteristicValue);
                // Get average sensor values
                GetAverageSensorValues(CharacteristicValue);
                // Get unfiltered (current) sensor values
                GetUnfilteredSensorValues(CharacteristicValue);
                // Get unfiltered floating point (current) sensor values
                GetUnfilteredFloatingPointSensorValues(CharacteristicValue);
                
                // Notify property changed
                RaisePropertyChanged(() => CharacteristicValue);
            }
            catch (Exception ex)
            {
                HockeyApp.MetricsManager.TrackEvent($"(CharacteristicOnValueUpdated) Message: {ex.Message}; StackTrace: {ex.StackTrace}");
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
        /// Serialize tab based sensor data to strongly typed Sensor
        /// </summary>
        /// <param name="sensorValues"></param>
        /// <param name="conversionType"></param>
        private void SerializeStringToSensor(string sensorValues, string conversionType)
        {
            // Split by tab delimiter
            string[] splitSensorValues = sensorValues.Split('\t');
            
            switch (conversionType)
            {
                case "A":
                    //Application.Current.MainPage.DisplayAlert("SensorIndex A", splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1), "Cancel");
                    //Application.Current.MainPage.DisplayAlert("SerialNumber", splitSensorValues[1], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("Name", splitSensorValues[2], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("SensorType", splitSensorValues[3], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("Scale", splitSensorValues[4], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("Offset", splitSensorValues[5], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("TimeStamp", splitSensorValues[6], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("AverageValue", splitSensorValues[7], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("CurrentValue", splitSensorValues[8], "Cancel");
                    //Application.Current.MainPage.DisplayAlert("DecimalLocation", splitSensorValues[9].SafeConvert<int>(0).ToString(), "Cancel");
                    //Application.Current.MainPage.DisplayAlert("StatisticsTotalCalcSettings", splitSensorValues[10], "Cancel");
                    
                    // "A" Sensor data serialization
                    var sensorListItemA = Sensors.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0));
                    if (sensorListItemA != null)
                    {
                        // Update sensor items in list
                        sensorListItemA.SensorIndex = splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0);
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
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("(A) DecimalLocation: ", splitSensorValues[9].SafeConvert<int>(0).ToString(), "Cancel");
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
                        Sensors.Add(sensor);
                    }
                    break;
                case "B":
                    // "B" Sensor data serialization
                    // Update Sensor list by index
                    var sensorListItemB = Sensors.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('B') + 1).SafeConvert<int>(0));
                    if (sensorListItemB != null)
                    {
                        Application.Current.MainPage.DisplayAlert("(B) AverageValue: ", splitSensorValues[2].SafeHexToInt().ToString(), "Cancel");
                        Application.Current.MainPage.DisplayAlert("(B) DecimalLocation: ", splitSensorValues[2].SafeHexToInt().ToString(), "Cancel");

                        sensorListItemB.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorListItemB.AverageValue = splitSensorValues[1].SafeHexToDouble();
                        sensorListItemB.DecimalLocation = splitSensorValues[2].SafeHexToInt();
                        sensorListItemB.AlarmStatus = splitSensorValues[3].SafeHexToInt();
                    }
                    RaisePropertyChanged(() => Sensors);
                    break;
                case "C":
                    //Application.Current.MainPage.DisplayAlert("C", splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0).ToString(), "Cancel");
                    // "C" Sensor data serialization
                    // Update Sensor list by index
                    var sensorListItemC = Sensors.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('C') + 1).SafeConvert<int>(0));
                    if (sensorListItemC != null)
                    {
                        //Application.Current.MainPage.DisplayAlert("Old value: ", sensorListItem.AverageValue.ToString(), "Cancel");
                        //Application.Current.MainPage.DisplayAlert("New value: ", splitSensorValues[1].SafeHexToDouble().ToString(), "Cancel");

                        sensorListItemC.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorListItemC.CurrentValue = splitSensorValues[1].SafeHexToDouble();
                    }
                    RaisePropertyChanged(() => Sensors);
                    break;
                case "I":
                    //Application.Current.MainPage.DisplayAlert("I", splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0).ToString(), "Cancel");
                    // "I" Sensor data serialization
                    // Update Sensor list by index
                    var sensorListItemI = Sensors.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('C') + 1).SafeConvert<int>(0));
                    if (sensorListItemI != null)
                    {
                        //Application.Current.MainPage.DisplayAlert("Old value: ", sensorListItem.AverageValue.ToString(), "Cancel");
                        //Application.Current.MainPage.DisplayAlert("New value: ", splitSensorValues[1].SafeHexToDouble().ToString(), "Cancel");

                        sensorListItemI.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorListItemI.CurrentValue = splitSensorValues[1].SafeHexToDouble();
                    }
                    RaisePropertyChanged(() => Sensors);
                    break;
                case "F":
                    //Application.Current.MainPage.DisplayAlert("I", splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('A') + 1).SafeConvert<int>(0).ToString(), "Cancel");
                    // "I" Sensor data serialization
                    // Update Sensor list by index
                    var sensorListItemF = Sensors.FirstOrDefault(s => s.SensorIndex == splitSensorValues[0].Substring(splitSensorValues[0].LastIndexOf('C') + 1).SafeConvert<int>(0));
                    if (sensorListItemF != null)
                    {
                        //Application.Current.MainPage.DisplayAlert("Old value: ", sensorListItem.AverageValue.ToString(), "Cancel");
                        //Application.Current.MainPage.DisplayAlert("New value: ", splitSensorValues[1].SafeHexToDouble().ToString(), "Cancel");

                        sensorListItemF.TimeStamp = splitSensorValues[0].SafeHexToInt();
                        sensorListItemF.CurrentValue = splitSensorValues[1].SafeHexToDouble();
                    }
                    RaisePropertyChanged(() => Sensors);
                    break;
                default:
                    throw new Exception($"nameof(conversionType) not defined");
            }
        }

        /// <summary>
        /// Sensor selected (navigate to sensor plot page)
        /// </summary>
        public IService SelectedSensor
        {
            get => null;
            set
            {
                if (value != null)
                {
                    var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { SensorIdKey, value.Id.ToString() } });
                    // Navigate to sensor plot
                    ShowViewModel<SensorPlotViewModel>(bundle);
                }
                RaisePropertyChanged();
            }
        }

        
    }
}