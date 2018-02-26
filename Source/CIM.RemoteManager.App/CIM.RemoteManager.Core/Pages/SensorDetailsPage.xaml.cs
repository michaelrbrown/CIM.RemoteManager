using System;
using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorPlotPage
    {
        /// <summary>
        /// Sensor
        /// </summary>
        public Sensor Sensor { get; set; }

        public SensorPlotPage()
        {
            InitializeComponent();
            BindingContext = this;

            this.CurrentPageChanged += CurrentPageHasChanged;

            // Add sensor statistics tabbed page
            //var sensorStatisticsPage = new NavigationPage(new SensorStatisticsPage())
            //{
            //    Title = "Sensor Statistics",
            //    Icon = "ic_sensor-statistics.png"
            //};
            //Children.Add(sensorStatisticsPage);



            //SensorBusyIndicator.IsBusy = false;
        }

        protected override void OnCurrentPageChanged()
        {
            if (this.CurrentPage is SensorStatisticsPage)
            {
                //DisplayAlert("SensorStats", "onapprea", "Ok");
                //sensorPlotPage.BindingContext = sensorItem;
                //var viewModel = BindingContext as SensorPlotViewModel;

                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Set sensor command type to pull Statistics Characteristics
                sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Statistics;
                sensorDetailsViewModel?.StartUpdatesCommand.Execute();

                // Send our Sensor object as message
                //var message = new SensorMessage(this, sensorPlotViewModel.Sensor);

                //var sensorStatisticsViewModel = (SensorStatisticsViewModel)this.BindingContext;

                // Publish our message
                //MvxMessenger.Publish<SensorMessage>(message);


                //DisplayAlert("SensorStts", sensorPlotViewModel?.Sensor.SensorIndex.ToString(), "Ok");


                //sensorPlotViewModel?.NavigateToSensorStatisticsPage(sensorPlotViewModel?.Sensor);
            }
            else if (this.CurrentPage is SensorPlotPage)
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Set sensor command type to pull Statistics Characteristics
                sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
                sensorDetailsViewModel?.StartUpdatesCommand.Execute();
            }
        }

        private void CurrentPageHasChanged(object sender, EventArgs e)
        {
            this.Title = this.CurrentPage.Title;
        }

        /// <summary>
        /// Handle toggling of sensor updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorUpdatesSwitchToggled(object sender, ToggledEventArgs e)
        {
            // Get instance of SensorPlotViewModel
            var sensorPlotViewModel = (SensorDetailsViewModel)this.BindingContext;
            // Toggle sensor updates
            sensorPlotViewModel.ToggleUpdatesCommand.Execute(null);
        }

    }
}