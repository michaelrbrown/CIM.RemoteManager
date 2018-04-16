using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms.Xaml;

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

            // Add device settings toolbar icon and handle selection
            //ToolbarItems.Add(new ToolbarItem("Device Settings", "nav_RemoteSettings.png", () =>
            //{
            //}, ToolbarItemOrder.Primary, 0));
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
            
            //if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            //{
            //    SensorListView.LayoutDefinition.Orientation = Telerik.XamarinForms.Common.Orientation.Vertical;
            //}
            //else if (Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet)
            //{
            //    SensorListView.LayoutDefinition.Orientation = Telerik.XamarinForms.Common.Orientation.Horizontal;
            //}
        }

        /// <summary>
        /// Handles the ItemTapped event of the listView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="itemTappedEventArgs">The <see cref="Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs"/> instance containing the event data.</param>
        private void listView_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs itemTappedEventArgs)
        {
            //var sensorPlotPage = new SensorPlotPage();
            if (itemTappedEventArgs.Item is Sensor sensorItem)
            {
                //sensorPlotPage.BindingContext = sensorItem;
                var sensorListViewModel = (SensorListViewModel)this.BindingContext;
                // Navigate to sensor details view model (for plot, statistics, limits, and sensor settings)
                sensorListViewModel.NavigateToSensorDetailsPage(sensorItem);
            }
        }
    }
}