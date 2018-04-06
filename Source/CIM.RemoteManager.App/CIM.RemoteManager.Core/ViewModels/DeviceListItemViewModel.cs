using System;
using MvvmCross.Core.ViewModels;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.ViewModels
{
    /// <summary>
    /// Class DeviceListItemViewModel.
    /// </summary>
    /// <seealso cref="MvvmCross.Core.ViewModels.MvxNotifyPropertyChanged" />
    public class DeviceListItemViewModel : MvxNotifyPropertyChanged
    {
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>The device.</value>
        public IDevice Device { get; private set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id => Device.Id;
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected => Device.State == DeviceState.Connected;
        /// <summary>
        /// Gets the rssi.
        /// </summary>
        /// <value>The rssi.</value>
        public int Rssi => Device.Rssi;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => Device.Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceListItemViewModel"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceListItemViewModel(IDevice device)
        {
            Device = device;
        }

        /// <summary>
        /// Updates the specified new device.
        /// </summary>
        /// <param name="newDevice">The new device.</param>
        public void Update(IDevice newDevice = null)
        {
            if (newDevice != null)
            {
                Device = newDevice;
            }
            RaisePropertyChanged(nameof(IsConnected));
            RaisePropertyChanged(nameof(Rssi));
        }
    }
}
