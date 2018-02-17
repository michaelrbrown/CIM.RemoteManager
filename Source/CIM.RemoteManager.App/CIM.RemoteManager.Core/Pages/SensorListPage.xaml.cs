using System;
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
            BindingContext = this; // Note that I added this line
        }
        
        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            
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
        /// <param name="e"></param>
        private void SensorLiistView_OnItemDoubleTapped(object sender, ItemDoubleTappedEventArgs e)
        {
            DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor) e.ItemData).SensorIndex, "OK");
        }

        private void ListView_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            if (e.SwipeOffset >= 360)
            {
                //SensorPlotViewModel sensorPlotViewModel;
                //BindingContext = (sensorPlotViewModel = new SensorPlotViewModel());
                //sensorPlotViewModel.(e.ItemIndex);
                //SensorLiistView.ResetSwipe();
            }
        }

    }
}