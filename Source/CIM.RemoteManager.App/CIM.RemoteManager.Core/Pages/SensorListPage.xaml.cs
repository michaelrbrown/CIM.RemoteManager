using System;
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
            
            //SensorBusyIndicator.IsBusy = false;
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            
        }

        private void ListView_OnStats(object sender, EventArgs e)
        {
            
        }
   
        private void ListView_OnSettings(object sender, EventArgs e)
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
            sensorListViewModel.ToggleUpdatesCommand.Execute(null);
        }

        private void ListView_OnScrollStateChanged(object sender, ScrollStateChangedEventArgs e)
        {
            
        }

        private void SensorLiistView_OnItemDoubleTapped(object sender, ItemDoubleTappedEventArgs e)
        {
            //DisplayAlert("Alert", "Selected Book number: " + (e.ItemData as SensorPlotViewModel)., "OK");
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