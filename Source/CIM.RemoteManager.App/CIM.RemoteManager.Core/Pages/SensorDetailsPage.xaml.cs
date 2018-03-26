using System;
using System.ServiceModel.Dispatcher;
using CIM.RemoteManager.Core.Helpers;
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

            // Get instance of sensor details viewmodel
            var sensorDetailsViewModel = this.BindingContext as SensorDetailsViewModel;

            // Add device settings toolbar icon and handle selection
            ToolbarItems.Add(new ToolbarItem("Refresh Sensor Data", "ic_remote-settings.png", () =>
            {
                // Validate
                if (sensorDetailsViewModel != null)
                {
                    // Refresh sensor data
                    sensorDetailsViewModel.StopUpdatesCommand.Execute();
                    sensorDetailsViewModel.StartUpdatesCommand.Execute();
                }
            }, ToolbarItemOrder.Primary, 0));

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



        }

        /// <summary>
        /// Tabbed current page changed event to handle setting sensor update type.
        /// Also handles starting sensor updates based on type.
        /// </summary>
        protected override void OnCurrentPageChanged()
        {
            if (this.CurrentPage.Title == "Sensor Statistics")
            {
                // Get binding context of Sensor Details viewmodel
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
                // Get binding context of Sensor Details viewmodel
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
                // Get binding context of Sensor Details viewmodel
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

        /// <summary>
        /// Offset trim switch toggled.
        /// </summary>
        /// <remarks>
        /// Disables or enables Lower Calibration, Lower Calibration Target, and Scale.
        /// </remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ToggledEventArgs"/> instance containing the event data.</param>
        private void SensorOffsetTrimSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                LowerCalibrationEntry.IsEnabled = false;
                LowerCalibrationTargetEntry.IsEnabled = false;
                ScaleEntry.IsEnabled = false;
            }
            else
            {
                LowerCalibrationEntry.IsEnabled = true;
                LowerCalibrationTargetEntry.IsEnabled = true;
                ScaleEntry.IsEnabled = true;
            }
        }

        /// <summary>
        /// The scale and offset entry calibration fields.
        /// </summary>
        private string _upperCalibrationEntryValue = "";
        private string _upperCalibrationTargetEntryValue = "";
        private string _lowerCalibrationEntryValue = "";
        private string _lowerCalibrationTargetEntryalue = "";

        private void UpperCalibrationEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _upperCalibrationEntryValue = e.NewTextValue;
        }

        private void UpperCalibrationTargetEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _upperCalibrationTargetEntryValue = e.NewTextValue;
        }

        private void LowerCalibrationEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _lowerCalibrationEntryValue = e.NewTextValue;
        }

        private void LowerCalibrationTargetEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _lowerCalibrationTargetEntryalue = e.NewTextValue;
        }

        /// <summary>
        /// Calculates the scale and offset.
        /// </summary>
        private void CalculateScaleAndOffset()
        {
            try
            {
                // Get entry values for calculation
                double upperCalibrationEntryValue = _upperCalibrationEntryValue.SafeConvert<double>(0);
                double upperCalibrationTargetEntryValue = _upperCalibrationTargetEntryValue.SafeConvert<double>(0);
                double lowerCalibrationEntryValue = _lowerCalibrationEntryValue.SafeConvert<double>(0);
                double lowerCalibrationTargetEntryalue = _lowerCalibrationTargetEntryalue.SafeConvert<double>(0);

                // Calculate x and y
                double y = upperCalibrationTargetEntryValue - lowerCalibrationTargetEntryalue;
                double x = upperCalibrationEntryValue - lowerCalibrationEntryValue;

                // Calculate scale
                double scale = y / x;
                // Calculate offset
                double offset = upperCalibrationTargetEntryValue - (scale * upperCalibrationEntryValue);

                // Show value in scale entry
                ScaleEntry.Text = scale.ToString();

                // Show value in offset entry
                OffsetEntry.Text = offset.ToString();
            }
            catch
            {
                // ignored
            }
        }

    }
}