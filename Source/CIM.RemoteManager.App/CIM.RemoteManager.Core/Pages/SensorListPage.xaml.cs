using System;
using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Syncfusion.ListView.XForms;
using Syncfusion.SfBusyIndicator.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
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
            ToolbarItems.Add(new ToolbarItem("Device Settings", "ic_RemoteSettings.png", () =>
            {
            }, ToolbarItemOrder.Primary, 0));

            SfBusyIndicator busyIndicator = new SfBusyIndicator
            {
                AnimationType = AnimationTypes.HorizontalPulsingBox,
                ViewBoxWidth = 150,
                ViewBoxHeight = 150,
                TextColor = Color.FromHex("#01649D")
            };
            this.Content = busyIndicator;
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

            //// Get instance of SensorListViewModel
            //var sensorListViewModel = (SensorListViewModel)this.BindingContext;
            //// Validate
            //if (sensorListViewModel != null)
            //{
            //    sensorListViewModel.StopUpdatesCommand.Execute();
            //    sensorListViewModel.StartUpdatesCommand.Execute();
            //}
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

        /// <summary>
        /// Handles the OnRefreshing event of the PullToRefresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PullToRefresh_OnRefreshing(object sender, EventArgs e)
        {
            // Get instance of SensorListViewModel
            //var sensorListViewModel = (SensorListViewModel)this.BindingContext;
            //// Validate
            //if (sensorListViewModel != null)
            //{
            //    sensorListViewModel.StopUpdatesCommand.Execute();
            //    sensorListViewModel.StartUpdatesCommand.Execute();
            //}
        }
    }
}