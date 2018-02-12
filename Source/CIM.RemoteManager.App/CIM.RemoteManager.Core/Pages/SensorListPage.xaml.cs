using System;
using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage : ContentPage
    {
        public SensorListPage()
        {
            InitializeComponent();
            BindingContext = this; // Note that I added this line

            SensorBusyIndicator.IsBusy = false;
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
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
    }
}