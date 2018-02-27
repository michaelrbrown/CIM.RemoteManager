using System;
using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorDetailsPage
    {
        /// <summary>
        /// Sensor
        /// </summary>
        public Sensor Sensor { get; set; }

        public SensorDetailsPage()
        {
            InitializeComponent();

            // Set binding context to viewmodel
            BindingContext = this;

            // Set current paged changed event to handle sensor update types
            this.CurrentPageChanged += CurrentPageHasChanged;
        }

        /// <summary>
        /// Tabbed current page changed event to handle setting sensor update type.
        /// Also handles starting sensor updates based on type.
        /// </summary>
        protected override void OnCurrentPageChanged()
        {
            if (this.CurrentPage.Title == "Sensor Statistics")
            {
                

                DisplayAlert("details page type", "sensor details page", "ok");

                // Send our Sensor object as message
                //var message = new SensorMessage(this, SensorDetailsViewModel.SensorCommand.Statistics);
                // Publish our message
                //MvxMessenger.Publish<SensorMessage>(message);

                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Set sensor command type to pull Statistics Characteristics
                //sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Statistics;
                //sensorDetailsViewModel?.StopUpdatesCommand.Execute();
                //sensorDetailsViewModel?.SetSensorCommandType(SensorDetailsViewModel.SensorCommand.Statistics);
                //sensorDetailsViewModel?.StartUpdatesCommand.Execute();
            }
            else
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;

                // Send our Sensor object as message
                //var message = new SensorMessage(this, SensorDetailsViewModel.SensorCommand.Statistics);
                // Publish our message
                //MvxMessenger.Publish<SensorMessage>(message);

                // Set sensor command type to pull Statistics Characteristics
                //sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
                //sensorDetailsViewModel?.StopUpdatesCommand.Execute();
                //sensorDetailsViewModel?.SetSensorCommandType(SensorDetailsViewModel.SensorCommand.Plot);
                //sensorDetailsViewModel?.StartUpdatesCommand.Execute();
            }
        }

        /// <summary>
        /// Tabbed current page change (handle setting page title)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
            // Toggle sensor updates
            sensorDetailsViewModel.ToggleUpdatesCommand.Execute(null);
        }

    }
}