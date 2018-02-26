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
            if (this.CurrentPage is SensorStatisticsPage)
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Set sensor command type to pull Statistics Characteristics
                sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Statistics;
                sensorDetailsViewModel?.StopUpdatesCommand.Execute();
                sensorDetailsViewModel?.StartSensorUpdates(SensorDetailsViewModel.SensorCommand.Statistics);
            }
            else if (this.CurrentPage is SensorDetailsPage)
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Set sensor command type to pull Statistics Characteristics
                sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
                sensorDetailsViewModel?.StopUpdatesCommand.Execute();
                sensorDetailsViewModel?.StartSensorUpdates(SensorDetailsViewModel.SensorCommand.Plot);
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