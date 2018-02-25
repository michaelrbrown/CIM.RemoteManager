using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage
    {
        private IMvxMessenger _messenger;

        public SensorListPage()
        {
            InitializeComponent();
            // Set binding context
            BindingContext = this;

            ToolbarItems.Add(new ToolbarItem("Device Settings", "ic_remote-settings.png", () =>
            {
                
            }));
        }
        
        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
        {
            //DisplayAlert("Item Tapped", "", "OK");
            //DisplayAlert("Item Tapped", "Sensor: " + itemTappedEventArgs.ItemData, "OK");
            //DisplayAlert("Item Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SensorIndex, "OK");
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
        private void SensorLiistView_OnItemDoubleTapped(object sender, ItemDoubleTappedEventArgs itemTappedEventArgs)
        {
            //var sensorPlotPage = new SensorPlotPage();
            if (itemTappedEventArgs.ItemData is Sensor sensorItem)
            {
                // Send our Sensor object as message
                var message = new SensorMessage(this, sensorItem);
                // Publish our message
                _messenger.Publish(message);

                //sensorPlotPage.BindingContext = sensorItem;
                var sensorListViewModel = (SensorListViewModel)this.BindingContext;
                sensorListViewModel.NavigateToSensorPlotPage(sensorItem);
                //sensorListViewModel.NavigateToSensorPlotPage(((Sensor)itemTappedEventArgs.ItemData).SerialNumber);
            }

            // Get instance of SensorListViewModel
            

            //DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SerialNumber, "OK");

            
            //Navigation.PushAsync(new SensorPlotPage());




            //DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SensorIndex, "OK");


            // DisplayAlert("Item Tapped", "Sensor SerialNumber: " + ((Sensor)itemTappedEventArgs.ItemData).SerialNumber, "OK");

            //var sensorItem = itemTappedEventArgs.ItemData as Sensor;

            //if (sensorItem != null) DisplayAlert("Item Tapped", "Sensor SensorType: " + sensorItem.SensorType, "OK");

            //if (itemTappedEventArgs.ItemData != null)
            // DisplayAlert("Item Tapped", "Sensor: " + itemTappedEventArgs.ItemData.ToString(), "OK");
            // Navigate to sensor plot page passing bundle
            //sensorListViewModel.NavigateToSensorPlotPage(item.SensorIndex.ToString());
            //Navigation.PushAsync(sensorPlotPage);
            //}

            //DisplayAlert("Item Double Tapped", "", "OK");
            //DisplayAlert("Item Double Tapped", "Sensor: " + itemTappedEventArgs.ItemData, "OK");
            //DisplayAlert("Item Double Tapped", "Sensor Index: " + ((Sensor)itemTappedEventArgs.ItemData).SensorIndex, "OK");
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