using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CIM.RemoteManager.Core.Helpers;
using CIM.RemoteManager.Core.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions.Contracts;


namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorListViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private IDevice _device;

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

        private IList<ISensor> _sensors;

        public IList<ISensor> Sensors
        {
            get => _sensors;
            private set => SetProperty(ref _sensors, value);
        }

        public SensorListViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;


            string message = "{A }";

            WriteValueAsync(message);

        }

        public override void Resume()
        {
            base.Resume();

            LoadSensorData();
        }

        private async void LoadSensorData()
        {
            try
            {
                _userDialogs.ShowLoading("Loading sensor data...");

                Guid deviceGuid = _device.Id;
                
                Sensors = await GetSensorsAsync();
                
                RaisePropertyChanged(() => Sensors);

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

        private async void WriteValueAsync(string characteristicValue)
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
                var result = await _userDialogs.PromptAsync("Input a value", "Write value", placeholder: characteristicValue);

                if (!result.Ok)
                    return;


                //var data = GetBytes(result.Text);

                // Show loading indicator
                _userDialogs.ShowLoading("Write characteristic value");


                // Get our adafruit bluetooth service
                var service = await _device.GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));

                // Get our adafruit bluetooth characteristic
                // Tx (Write)
                _tx = await service.GetCharacteristicAsync(TxUuid);

                // Get our adafruit bluetooth characteristic
                // RX (read)
                //var characteristicRead = await service.GetCharacteristicAsync(RxUuid);

                // Write values async
                await _tx.WriteAsync(characteristicValue.StrToByteArray());

                // Hide loading...
                _userDialogs.HideLoading();

                //RaisePropertyChanged(() => CharacteristicValue);

                _userDialogs.Toast($"Wrote value {characteristicValue}");

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

            if (_device == null)
            {
                Close(this);
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