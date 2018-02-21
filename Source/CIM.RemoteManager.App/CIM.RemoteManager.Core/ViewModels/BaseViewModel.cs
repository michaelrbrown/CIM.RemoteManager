using System;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        protected readonly IAdapter Adapter;
        protected const string DeviceIdKey = "DeviceIdNavigationKey";
        protected const string ServiceIdKey = "ServiceIdNavigationKey";
        protected const string CharacteristicIdKey = "CharacteristicIdNavigationKey";
        protected const string DescriptorIdKey = "DescriptorIdNavigationKey";
        protected const string SensorIdKey = "SensorIdNavigationKey";

        /// <summary>
        /// UUIDs for UART service and associated characteristics.
        /// </summary>
        protected static Guid UartUuid = Guid.Parse("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");

        protected static Guid TxUuid = Guid.Parse("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        protected static Guid RxUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        /// <summary>
        /// UUID for the UART BTLE client characteristic which is necessary for notifications.
        /// </summary>
        public static Guid ClientUuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        /// <summary>
        /// UUIDs for the Device Information service and associated characteristics.
        /// </summary>
        public static Guid DisUuid = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");

        public static Guid DisManufUuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        public static Guid DisModelUuid = Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb");
        public static Guid DisHwrevUuid = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");
        public static Guid DisSwrevUuid = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");

        public BaseViewModel(IAdapter adapter)
        {
            Adapter = adapter;
        }

        public virtual void Resume()
        {
            Mvx.Trace("Resume {0}", GetType().Name);
        }

        public virtual void Suspend()
        {
            Mvx.Trace("Suspend {0}", GetType().Name);
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            Bundle = parameters;
        }

        protected IMvxBundle Bundle { get; private set; }

        protected IDevice GetDeviceFromBundle(IMvxBundle parameters)
        {
            if (!parameters.Data.ContainsKey(DeviceIdKey)) return null;
            var deviceId = parameters.Data[DeviceIdKey];

            return Adapter.ConnectedDevices.FirstOrDefault(d => d.Id.ToString().Equals(deviceId));
        }

        protected IDevice GetSensorDeviceBundle(IMvxBundle parameters)
        {
            var deviceId = parameters.Data[DeviceIdKey];

            return Adapter.ConnectedDevices.FirstOrDefault(d => d.Id.ToString().Equals(deviceId));
        }

        protected Task<IService> GetServiceFromBundleAsync(IMvxBundle parameters)
        {

            var device = GetDeviceFromBundle(parameters);
            if (device == null || !parameters.Data.ContainsKey(ServiceIdKey))
            {
                return Task.FromResult<IService>(null);
            }

            var serviceId = parameters.Data[ServiceIdKey];
            return device.GetServiceAsync(Guid.Parse(serviceId));
        }

        protected async Task<ICharacteristic> GetCharacteristicFromBundleAsync(IMvxBundle parameters)
        {
            var service = await GetServiceFromBundleAsync(parameters);
            if (service == null || !parameters.Data.ContainsKey(CharacteristicIdKey))
            {
                return null;
            }

            var characteristicId = parameters.Data[CharacteristicIdKey];
            return await service.GetCharacteristicAsync(Guid.Parse(characteristicId));
        }
        
        protected async Task<IDescriptor> GetDescriptorFromBundleAsync(IMvxBundle parameters)
        {
            var characteristic = await GetCharacteristicFromBundleAsync(parameters);
            if (characteristic == null || !parameters.Data.ContainsKey(DescriptorIdKey))
            {
                return null;
            }

            var descriptorId = parameters.Data[DescriptorIdKey];
            return await characteristic.GetDescriptorAsync(Guid.Parse(descriptorId));
        }
    }
}