using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace CIM.RemoteManager.Core.Pages
{
    /// <summary>
    /// Class SensorListPage.
    /// </summary>
    /// <seealso cref="CIM.RemoteManager.Core.Pages.BasePage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorListPage"/> class.
        /// </summary>
        public SensorListPage()
        {
            InitializeComponent();

            // Set binding context to viewmodel
            BindingContext = this;

            // Add extra padding for iPhoneX
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            // Add device settings toolbar icon and handle selection
            ToolbarItems.Add(new ToolbarItem("Device Settings", "ic_remote-settings.png", () =>
            {
            }, ToolbarItemOrder.Primary, 0));
        }

        /// <summary>
        /// Called when [appearing].
        /// </summary>
        /// <remarks>
        /// Start updates in on appearing life cycle.
        /// </remarks>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Get instance of SensorListViewModel
            var sensorListViewModel = (SensorListViewModel)this.BindingContext;
            // Validate
            if (sensorListViewModel != null)
            {
                sensorListViewModel.StopUpdatesCommand.Execute();
                sensorListViewModel.StartUpdatesCommand.Execute();
            }
        }

        /// <summary>
        /// Handle toggling of sensor updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorUpdatesSwitchToggled(object sender, ToggledEventArgs e)
        {
            // Get instance of SensorListViewModel
            var sensorListViewModel = (SensorListViewModel)this.BindingContext;
            // Toggle sensor updates
            sensorListViewModel.ToggleUpdatesCommand.Execute(null);
        }

        /// <summary>
        /// Get sensor index on item double tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="itemTappedEventArgs"></param>
        private void SensorListView_OnItemDoubleTapped(object sender, ItemDoubleTappedEventArgs itemTappedEventArgs)
        {
            //var sensorPlotPage = new SensorPlotPage();
            if (itemTappedEventArgs.ItemData is Sensor sensorItem)
            {
                //sensorPlotPage.BindingContext = sensorItem;
                var sensorListViewModel = (SensorListViewModel)this.BindingContext;
                // Navigate to sensor details view model (for plot, statistics, limits, and sensor settings)
                sensorListViewModel.NavigateToSensorDetailsPage(sensorItem);
            }
        }

    }
}