using System;
using CIM.RemoteManager.Core.Models;
using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorDetailsPage
    {
        public SensorDetailsPage()
        {
            InitializeComponent();

            // Set binding context to viewmodel
            BindingContext = this;

            // Set current paged changed event to handle sensor update types
            this.CurrentPageChanged += CurrentPageHasChanged;

            //var sensorDetailsViewModel = this.BindingContext as SensorDetailsViewModel;
            //// Validate
            //if (sensorDetailsViewModel != null)
            //{
            //    sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
            //    sensorDetailsViewModel.StopUpdatesCommand.Execute();
            //    sensorDetailsViewModel.StartUpdatesCommand.Execute();

            //}

            // Get instance of sensor details viewmodel
            var sensorDetailsViewModel = this.BindingContext as SensorDetailsViewModel;
            // Add device settings toolbar icon and handle selection
            ToolbarItems.Add(new ToolbarItem("Refresh Sensor Data", "ic_refresh-sensordata.png", () =>
            {
                // Validate
                if (sensorDetailsViewModel != null)
                {
                    // Refresh sensor data
                    sensorDetailsViewModel.StopUpdatesCommand.Execute();
                    sensorDetailsViewModel.StartUpdatesCommand.Execute();
                }
            }, ToolbarItemOrder.Primary, 0));

        }

        /// <summary>
        /// Tabbed current page changed event to handle setting sensor update type.
        /// Also handles starting sensor updates based on type.
        /// </summary>
        protected override void OnCurrentPageChanged()
        {
            if (this.CurrentPage.Title == "Sensor Statistics")
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Validate
                if (sensorDetailsViewModel != null)
                {
                    // Set sensor command type to pull Statistics Characteristics
                    sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Statistics;
                    //sensorDetailsViewModel.StopUpdatesCommand.Execute();
                    //sensorDetailsViewModel.StartUpdatesCommand.Execute();
                }
            }
            if (this.CurrentPage.Title == "Sensor Limits")
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Validate
                if (sensorDetailsViewModel != null)
                {
                    // Set sensor command type to pull Statistics Characteristics
                    sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Limits;
                    //sensorDetailsViewModel.StopUpdatesCommand.Execute();
                    //sensorDetailsViewModel.StartUpdatesCommand.Execute();
                }
            }
            else
            {
                var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
                // Validate
                if (sensorDetailsViewModel != null)
                {
                    sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
                    sensorDetailsViewModel.StopUpdatesCommand.Execute();
                    sensorDetailsViewModel.StartUpdatesCommand.Execute();

                }
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