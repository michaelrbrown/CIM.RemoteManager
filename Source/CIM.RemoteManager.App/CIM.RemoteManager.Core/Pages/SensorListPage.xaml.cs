using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage : ContentPage
    {
        public SensorListPage()
        {
            InitializeComponent();
            // Set binding context
            BindingContext = this;
        }
        
        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            DisplayAlert("Item Double Tapped", "", "OK");
            DisplayAlert("Item Double Tapped", "Sensor: " + itemTappedEventArgs.ItemData, "OK");
            DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SensorIndex, "OK");
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
            SensorBusyIndicator.IsBusy = true;
            sensorListViewModel.ToggleUpdatesCommand.Execute(null);
            SensorBusyIndicator.IsBusy = false;
        }

        /// <summary>
        /// Get sensor index on item double tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="itemTappedEventArgs"></param>
        private void SensorLiistView_OnItemDoubleTapped(object sender, ItemDoubleTappedEventArgs itemTappedEventArgs)
        {
            DisplayAlert("Item Double Tapped", "", "OK");
            DisplayAlert("Item Double Tapped", "Sensor: " + itemTappedEventArgs.ItemData, "OK");
            DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SensorIndex, "OK");
        }

        private void ListView_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            if (e.SwipeOffset >= 360)
            {
                DisplayAlert("Item swipe ended...", "", "OK");
                //SensorPlotViewModel sensorPlotViewModel;
                //BindingContext = (sensorPlotViewModel = new SensorPlotViewModel());
                //sensorPlotViewModel.(e.ItemIndex);
                //SensorLiistView.ResetSwipe();
            }
        }

    }
}