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
            
            //SensorBusyIndicator.IsBusy = false;
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
        /// Handle the scrolled event raised by the <see cref="Controls.Views.ListView"/>.
        /// </summary>
        /// <param name="sender">The sender that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnListViewScrolled(object sender, ScrolledEventArgs args)
        {
            _userDialogs.Alert($"Offset: {args.ScrollY}", "CIMScan RemoteManager");
            ScrollOffset.Text = $"Offset = {args.ScrollY}";
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