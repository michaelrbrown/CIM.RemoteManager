﻿using Acr.UserDialogs;
using CIM.RemoteManager.Core.Extensions;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.ViewModels
{
    /// <summary>
    /// Device list viewmodel for handling connecting to BlueTooth devices, sorting and listing CimScan devices
    /// in a separate list.  Connections and disposing, as well as navigating to sensor list page all contained
    /// within the logic of this viewmodel. Inherits from the base viewmodel for shared core functions such as
    /// device bundle identifiers, suspending and resuming connections.
    /// </summary>
    /// <seealso cref="CIM.RemoteManager.Core.ViewModels.BaseViewModel" />
    public class DeviceListViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Bluetooth LE permissions.
        /// </summary>
        readonly IPermissions _permissions;
        /// <summary>
        /// Bluetooth LE device.
        /// </summary>
        private readonly IBluetoothLE _bluetoothLe;
        /// <summary>
        /// User dialogs.
        /// </summary>
        private readonly IUserDialogs _userDialogs;
        /// <summary>
        /// App Settings.
        /// </summary>
        private readonly ISettings _settings;
        /// <summary>
        /// Previous Guid.
        /// </summary>
        private Guid _previousGuid;
        /// <summary>
        /// Previous Name.
        /// </summary>
        private string _previousName = string.Empty;
        /// <summary>
        /// Cancellation token source (for async issues).
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Previous device guid.
        /// </summary>
        public Guid PreviousGuid
        {
            get => _previousGuid;
            set
            {
                _previousGuid = value;
                _settings.AddOrUpdateValue("lastguid", _previousGuid.ToString());
                RaisePropertyChanged();
                RaisePropertyChanged(() => ConnectToPreviousCommand);
            }
        }

        /// <summary>
        /// Previous device name.
        /// </summary>
        public string PreviousName
        {
            get => _previousName;
            set
            {
                _previousName = value;
                _settings.AddOrUpdateValue("lastname", _previousName.ToString());
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// All other devices ObservableCollection.
        /// </summary>
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        /// <summary>
        /// CIMScan ObservableCollection.
        /// </summary>
        public ObservableCollection<DeviceListItemViewModel> SystemDevices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();

        /// <summary>
        /// Are we refreshing?
        /// </summary>
        public bool IsRefreshing => Adapter.IsScanning;
        /// <summary>
        /// Is Bluetooth LE state on?
        /// </summary>
        public bool IsStateOn => _bluetoothLe.IsOn;
        /// <summary>
        /// Bluetooth LE states
        /// </summary>
        public string StateText => GetStateText();
        /// <summary>
        /// Did we find a system device (CIMScan device)?
        /// </summary>
        public bool FoundSystemDevices => SystemDevices.Any();
        /// <summary>
        /// Can connect to previous device.
        /// </summary>
        public bool CanConnectToPreviousDevice => _bluetoothLe.IsOn && PreviousGuid != Guid.Empty;
        /// <summary>
        /// Selected device.
        /// </summary>
        public DeviceListItemViewModel SelectedDevice
        {
            get => null;
            set
            {
                if (value != null)
                {
                    HandleSelectedDevice(value);
                }

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Use Android auto connect.
        /// </summary>
        bool _useAutoConnect;
        public bool UseAutoConnect
        {
            get => _useAutoConnect;

            set
            {
                if (_useAutoConnect == value)
                    return;

                _useAutoConnect = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Refresh searching command.
        /// </summary>
        public MvxCommand RefreshCommand => new MvxCommand(() => TryStartScanning(true));
        /// <summary>
        /// Disconnect device command.
        /// </summary>
        public MvxCommand<DeviceListItemViewModel> DisconnectCommand => new MvxCommand<DeviceListItemViewModel>(DisconnectDevice);

        // TODO: remove after finished debugging
        public MvxCommand<DeviceListItemViewModel> ConnectDisposeCommand => new MvxCommand<DeviceListItemViewModel>(ConnectAndDisposeDevice);

        /// <summary>
        /// Stop scanning command
        /// </summary>
        /// <value>
        /// The stop scan command.
        /// </value>
        public MvxCommand StopScanCommand => new MvxCommand(() =>
        {
            _cancellationTokenSource.Cancel();
            CleanupCancellationToken();
            RaisePropertyChanged(() => IsRefreshing);
        }, () => _cancellationTokenSource != null);

        /// <summary>
        /// Gets the copy unique identifier command.
        /// </summary>
        /// <value>
        /// The copy unique identifier command.
        /// </value>
        public MvxCommand<DeviceListItemViewModel> CopyGuidCommand => new MvxCommand<DeviceListItemViewModel>(device =>
        {
            PreviousGuid = device.Id;
        });

        /// <summary>
        /// Command to connect to previous device.
        /// </summary>
        /// <value>
        /// The connect to previous command.
        /// </value>
        public MvxCommand ConnectToPreviousCommand => new MvxCommand(ConnectToPreviousDeviceAsync, CanConnectToPrevious);

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [device connection lost]. Event to handle Bluetooth LE device disconnects.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DeviceErrorEventArgs"/> instance containing the event data.</param>
        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            SystemDevices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            _userDialogs.HideLoading();
            _userDialogs.ErrorToast("Error", $" Connection LOST {e.Device.Name}", TimeSpan.FromMilliseconds(6000));
        }

        /// <summary>
        /// Called when [state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            RaisePropertyChanged(nameof(IsStateOn));
            RaisePropertyChanged(nameof(StateText));
            RaisePropertyChanged(nameof(CanConnectToPreviousDevice));
        }


        /// <summary>
        /// Handles the ScanTimeoutElapsed event of the Adapter control. Updates refreshing prop when timeout elapsed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => IsRefreshing);
            CleanupCancellationToken();
        }

        /// <summary>
        /// Called when [device discovered].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            try
            {
                // CIMScan devices
                if (args.Device.Name.IndexOf("adafruit", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    AddOrUpdateSystemDevice(args.Device);
                }
                else // All other devices
                {
                    AddOrUpdateDevice(args.Device);
                }
            }
            catch { }
        }

        /// <summary>
        /// Called when [device connected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        private void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _userDialogs.InfoToast($"Connected to {e.Device.Name}.", TimeSpan.FromSeconds(3));
        }

        /// <summary>
        /// Called when [device disconnected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DeviceEventArgs"/> instance containing the event data.</param>
        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            SystemDevices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            _userDialogs.HideLoading();
            _userDialogs.InfoToast($"Disconnected from {e.Device.Name}.", TimeSpan.FromSeconds(3));
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceListViewModel"/> class.
        /// Device list viewmodel constructor.
        /// </summary>
        /// <param name="bluetoothLe">The bluetooth le.</param>
        /// <param name="adapter">The adapter.</param>
        /// <param name="userDialogs">The user dialogs.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="permissions">The permissions.</param>
        public DeviceListViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) : base(adapter)
        {
            _permissions = permissions;
            _bluetoothLe = bluetoothLe;
            _userDialogs = userDialogs;
            _settings = settings;
            // Setup event handlers
            _bluetoothLe.StateChanged += OnStateChanged;
            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            Adapter.DeviceDisconnected += OnDeviceDisconnected;
            Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            Adapter.DeviceConnected += OnDeviceConnected;

            // Clear devices from list
            Devices.Clear();
            SystemDevices.Clear();

            // Kick off a scan on load
            TryStartScanning(false);
        }

        /// <summary>
        /// Resume searching for Bluetooth LE devices.
        /// </summary>
        public override async void Resume()
        {
            base.Resume();

            await GetPreviousGuidAsync().ConfigureAwait(true);
            await GetPreviousNameAsync().ConfigureAwait(true);
            TryStartScanning();
        }

        /// <summary>
        /// Stop scanning when in progress.
        /// </summary>
        public override void Suspend()
        {
            base.Suspend();

            Adapter.StopScanningForDevicesAsync();
            RaisePropertyChanged(() => IsRefreshing);
        }

        /// <summary>
        /// Tries the start scanning.
        /// </summary>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        private async void TryStartScanning(bool refresh = false)
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    var status = await _permissions.CheckPermissionStatusAsync(Permission.Location).ConfigureAwait(true);
                    if (status != PermissionStatus.Granted)
                    {
                        var permissionResult = await _permissions.RequestPermissionsAsync(Permission.Location).ConfigureAwait(true);

                        if (permissionResult.First().Value != PermissionStatus.Granted)
                        {
                            _userDialogs.ErrorToast("Error", " Permission denied. Not scanning.", TimeSpan.FromSeconds(5));
                            return;
                        }
                    }
                }

                if (IsStateOn && (refresh || !Devices.Any()) && !IsRefreshing)
                {
                    ScanForDevices();
                }
            }
            catch { }
        }

        /// <summary>
        /// Clear devices then start scanning through found devices and sort CIMScan from all others.
        /// </summary>
        private async void ScanForDevices()
        {
            try
            {
                // Clear devices from list
                Devices.Clear();
                SystemDevices.Clear();

                foreach (var connectedDevice in Adapter.ConnectedDevices)
                {
                    // update rssi for already connected devices (so the 0 is not shown in the list)
                    try
                    {
                        await connectedDevice.UpdateRssiAsync().ConfigureAwait(true);
                    }
                    catch (Exception ex)
                    {
                        Mvx.Trace(ex.Message);
                        _userDialogs.ErrorToast("Error", $" Failed to update RSSI for {connectedDevice.Name}", TimeSpan.FromSeconds(5));
                    }

                    // CIMScan devices
                    if (connectedDevice.Name.IndexOf("adafruit", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        AddOrUpdateSystemDevice(connectedDevice);
                    }
                    else // All other devices
                    {
                        AddOrUpdateDevice(connectedDevice);
                    }
                }

                // Set token for task cancellation handling
                _cancellationTokenSource = new CancellationTokenSource(5000);
                RaisePropertyChanged(() => StopScanCommand);
                // Notify we are refreshing
                RaisePropertyChanged(() => IsRefreshing);

                // Set scan mode to low latency which is the highest battery usage but best scan mode (possibly make adjustable)
                Adapter.ScanMode = ScanMode.LowLatency;
                await Adapter.StartScanningForDevicesAsync(_cancellationTokenSource.Token);
            }
            catch { }
        }

        /// <summary>
        /// Get previous connected device guid.
        /// </summary>
        /// <returns></returns>
        private Task GetPreviousGuidAsync()
        {
            return Task.Run(() =>
            {
                var guidString = _settings.GetValueOrDefault("lastguid", string.Empty);
                PreviousGuid = !string.IsNullOrEmpty(guidString) ? Guid.Parse(guidString) : Guid.Empty;
            });
        }

        /// <summary>
        /// Gets the previous name asynchronous.
        /// </summary>
        /// <returns>Task</returns>
        private Task GetPreviousNameAsync()
        {
            return Task.Run(() =>
            {
                var nameString = _settings.GetValueOrDefault("lastname", string.Empty);
                PreviousName = !string.IsNullOrEmpty(nameString) ? nameString : string.Empty;
            });
        }

        /// <summary>
        /// Gets the state text. Setup all the possible Bluetooth LE states.
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
        /// Add or update non-CIMScan devices in list (we want to do this b/c we may not find CIMScan
        /// devices by name (adafruit).
        /// </summary>
        /// <param name="device">The device.</param>
        private void AddOrUpdateDevice(IDevice device)
        {
            InvokeOnMainThread(() =>
            {
                var deviceListItemViewModel = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
                if (deviceListItemViewModel != null)
                {
                    deviceListItemViewModel.Update();
                }
                else
                {
                    bool deviceInList = false;
                    foreach (var currentDevice in Devices)
                    {
                        if (currentDevice.Id != device.Id) deviceInList = true;
                    }
                    // Only add deivce in list if not there
                    if (!deviceInList) Devices.Add(new DeviceListItemViewModel(device));
                }
            });
        }

        /// <summary>
        /// Add or update our CIMScan devices in list.
        /// </summary>
        /// <param name="device">The device.</param>
        private void AddOrUpdateSystemDevice(IDevice device)
        {
            InvokeOnMainThread(() =>
            {
                var deviceListItemViewModel = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
                if (deviceListItemViewModel != null)
                {
                    deviceListItemViewModel.Update();
                }
                else
                {
                    bool deviceInList = false;
                    foreach (var currentDevice in Devices)
                    {
                        if (currentDevice.Id != device.Id) deviceInList = true;
                    }
                    // Only add deivce in list if not there
                    if (!deviceInList) SystemDevices.Add(new DeviceListItemViewModel(device));
                }
            });
        }

        /// <summary>
        /// Cleanups the cancellation token.
        /// </summary>
        private void CleanupCancellationToken()
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            RaisePropertyChanged(() => StopScanCommand);
        }

        /// <summary>
        /// Disconnects the device.
        /// </summary>
        /// <param name="device">The device.</param>
        private async void DisconnectDevice(DeviceListItemViewModel device)
        {
            try
            {
                if (!device.IsConnected)
                    return;

                _userDialogs.ShowLoading($"Disconnecting {device.Name}...");

                await Adapter.DisconnectDeviceAsync(device.Device).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Disconnect error");
            }
            finally
            {
                device.Update();
                _userDialogs.HideLoading();
            }
        }

        /// <summary>
        /// Handles the selected device. Handle selected device by providing options to connect, disconnect, and navigate to sensors page.
        /// </summary>
        /// <param name="device">The device.</param>
        private void HandleSelectedDevice(DeviceListItemViewModel device)
        {
            // ActionSheet for handling selected device options
            var config = new ActionSheetConfig();

            if (device.IsConnected)
            {
                // We are already connected so give option to navigate back to sensors
                config.Add("Navigate to Sensors", () =>
                {
                    ShowViewModel<SensorListViewModel>(new MvxBundle(new Dictionary<string, string> { { DeviceIdKey, device.Device.Id.ToString() } }));
                });

                // Updates RSSI for selected device
                config.Add("Update RSSI", async () =>
                {
                    try
                    {
                        _userDialogs.ShowLoading();

                        await device.Device.UpdateRssiAsync().ConfigureAwait(true);
                        device.RaisePropertyChanged(nameof(device.Rssi));

                        _userDialogs.HideLoading();
                        _userDialogs.InfoToast($"RSSI updated {device.Rssi}.", TimeSpan.FromSeconds(2));
                    }
                    catch (Exception ex)
                    {
                        _userDialogs.HideLoading();
                        _userDialogs.ErrorToast("Error", $" Failed to update rssi. Exception: {ex.Message}", TimeSpan.FromSeconds(5));
                    }
                });
                // Disconnect from device
                config.Destructive = new ActionSheetOption("Disconnect", () => DisconnectCommand.Execute(device));
            }
            else
            {
                // Connect and navigate to sensors
                config.Add("Connect / Navigate to Sensors", async () =>
                {
                    if (await ConnectDeviceAsync(device).ConfigureAwait(true))
                    {
                        ShowViewModel<SensorListViewModel>(new MvxBundle(new Dictionary<string, string> { { DeviceIdKey, device.Device.Id.ToString() } }));
                    }
                });
                // TODO: remove after debugging phase...
                //config.Add("Connect & Dispose", () => ConnectDisposeCommand.Execute(device));
            }

            //config.Add("Copy GUID", () => CopyGuidCommand.Execute(device));
            config.Cancel = new ActionSheetOption("Cancel");
            config.SetTitle("Device Options");
            _userDialogs.ActionSheet(config);
        }

        /// <summary>
        /// Connects the device asynchronous. Handles connecting to device on selection.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="showPrompt">if set to <c>true</c> [show prompt].</param>
        /// <returns></returns>
        private async Task<bool> ConnectDeviceAsync(DeviceListItemViewModel device, bool showPrompt = true)
        {
            //if (showPrompt && !await _userDialogs.ConfirmAsync($"Connect to device '{device.Name}'?").ConfigureAwait(true))
            //{
            //    return false;
            //}
            try
            {
                var tokenSource = new CancellationTokenSource();

                // Setup progress dialog
                var config = new ProgressDialogConfig();
                if (!String.IsNullOrEmpty(device.Name))
                {
                    config.Title = $"Connecting to '{device.Name}'";
                    config.CancelText = "Cancel";
                    config.SetMaskType(MaskType.Black);
                    config.IsDeterministic = false;
                    config.OnCancel = tokenSource.Cancel;
                }
                else
                {
                    config.Title = $"Connecting to '{device.Id}'";
                    config.CancelText = "Cancel";
                    config.SetMaskType(MaskType.Black);
                    config.IsDeterministic = false;
                    config.OnCancel = tokenSource.Cancel;
                }

                // Progress dialog
                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();
                    // Set previous connected device info
                    PreviousGuid = device.Device.Id;
                    PreviousName = device.Device.Name;
                    await Adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token).ConfigureAwait(true);
                }

                //_userDialogs.InfoToast($"Connected to {device.Device.Name}.", TimeSpan.FromSeconds(3));

                return true;

            }
            catch
            {
                _userDialogs.Alert($"Cannot connect to device, ensure it's not in use.", "Connection error");
                return false;
            }
            finally
            {
                _userDialogs.HideLoading();
                device.Update();
            }
        }

        /// <summary>
        /// Handles connecting to previous device, which allows faster connection by eliminating searching and prompts.
        /// </summary>
        private async void ConnectToPreviousDeviceAsync()
        {
            // System device instance (CIMScan device)
            IDevice systemDevice;
            try
            {
                var tokenSource = new CancellationTokenSource();

                // Setup progress dialog
                var config = new ProgressDialogConfig()
                {
                    Title = $"Searching for '{PreviousName}'",
                    CancelText = "Cancel",
                    IsDeterministic = false,
                    OnCancel = tokenSource.Cancel
                };

                // Show progress while we connect
                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();
                    systemDevice = await Adapter.ConnectToKnownDeviceAsync(PreviousGuid, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token).ConfigureAwait(true);
                }

                // Notify end user of successful connection
                _userDialogs.InfoToast($"Connected to {systemDevice.Name}.", TimeSpan.FromSeconds(3));

                // Try to find our existing device by id
                var deviceItem = SystemDevices.FirstOrDefault(d => d.Device.Id == systemDevice.Id);
                if (deviceItem == null)
                {
                    // Add system device
                    deviceItem = new DeviceListItemViewModel(systemDevice);
                    Devices.Add(deviceItem);
                }
                else
                {
                    // Update system device
                    deviceItem.Update(systemDevice);
                }

                // Navigate to sensor list page
                ShowViewModel<SensorListViewModel>(new MvxBundle(new Dictionary<string, string> { { DeviceIdKey, deviceItem.Device.Id.ToString() } }));
            }
            catch (Exception ex)
            {
                _userDialogs.ErrorToast("Error", $" {ex.Message}", TimeSpan.FromSeconds(5));
                return;
            }
        }

        /// <summary>
        /// Can we connect to previous device?
        /// </summary>
        /// <returns></returns>
        private bool CanConnectToPrevious()
        {
            return PreviousGuid != default(Guid);
        }

        /// <summary>
        /// Connects to device and disposes after few seconds.  Mostly for debugging.
        /// </summary>
        /// <param name="item"></param>
        private async void ConnectAndDisposeDevice(DeviceListItemViewModel item)
        {
            try
            {
                using (item.Device)
                {
                    _userDialogs.ShowLoading($"Connecting to {item.Name} ...");
                    await Adapter.ConnectToDeviceAsync(item.Device).ConfigureAwait(true);

                    // TODO make this configurable
                    var resultMTU = await item.Device.RequestMtuAsync(60).ConfigureAwait(true);
                    System.Diagnostics.Debug.WriteLine($"Requested MTU. Result is {resultMTU}");

                    // TODO make this configurable
                    var resultInterval = item.Device.UpdateConnectionInterval(ConnectionInterval.High);
                    System.Diagnostics.Debug.WriteLine($"Set Connection Interval. Result is {resultInterval}");

                    item.Update();

                    _userDialogs.InfoToast($"Connected to {item.Device.Name}.", TimeSpan.FromSeconds(3));
                    _userDialogs.HideLoading();

                    for (var i = 5; i >= 1; i--)
                    {
                        _userDialogs.ShowLoading($"Disconnect in {i}s...");

                        await Task.Delay(1000).ConfigureAwait(true);

                        _userDialogs.HideLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Failed to connect and dispose.");
            }
            finally
            {
                _userDialogs.HideLoading();
            }
        }

    }
}