using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorPlotPage : TabbedPage
    {
        public SensorPlotPage()
        {
            InitializeComponent();
            BindingContext = this;

            // Add sensor statistics tabbed page
            var sensorStatisticsPage = new SensorStatisticsPage
            {
                Title = "Sensor Statistics",
                Icon = "ic_sensor-statistics.png"
            };
            Children.Add(sensorStatisticsPage);

            //SensorBusyIndicator.IsBusy = false;
        }

        /// <summary>
        /// Handle toggling of sensor updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorUpdatesSwitchToggled(object sender, ToggledEventArgs e)
        {
            // Get instance of SensorListViewModel
            //var sensorListViewModel = (SensorListViewModel)this.BindingContext;
            // Toggle sensor updates
            //sensorListViewModel.ToggleUpdatesCommand.Execute(null);
        }

    }
}