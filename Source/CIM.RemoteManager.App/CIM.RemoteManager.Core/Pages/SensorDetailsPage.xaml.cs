using CIM.RemoteManager.Core.Helpers;
using CIM.RemoteManager.Core.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ValueChangedEventArgs = SegmentedControl.FormsPlugin.Abstractions.ValueChangedEventArgs;

namespace CIM.RemoteManager.Core.Pages
{
    /// <summary>
    /// Class SensorDetailsPage.
    /// </summary>
    /// <seealso cref="CIM.RemoteManager.Core.Pages.BaseTabbedPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorDetailsPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorDetailsPage"/> class.
        /// </summary>
        public SensorDetailsPage()
        {
            InitializeComponent();

            // Set binding context to viewmodel
            BindingContext = this;

            // Set current paged changed event to handle sensor update types
            this.CurrentPageChanged += CurrentPageHasChanged;
        }

        /// <summary>
        /// When overridden, allows application developers to customize behavior immediately prior to the <see cref="T:Xamarin.Forms.Page" /> becoming visible.
        /// </summary>
        /// <remarks>
        /// Handles our view logic for on appearing life cycle events such as creating today event, moving to current service date, and initializing control defaults.
        /// </remarks>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Send plot command and start logging data
            if (this.BindingContext is SensorDetailsViewModel sensorDetailsViewModel)
            {
                sensorDetailsViewModel.SensorCommandType = SensorDetailsViewModel.SensorCommand.Plot;
                sensorDetailsViewModel.StopUpdatesCommand.Execute();
                sensorDetailsViewModel.StartUpdatesCommand.Execute();
            }
        }

        /// <summary>
        /// Tabbed current page changed event to handle setting sensor update type.
        /// Also handles starting sensor updates based on type.
        /// </summary>
        protected override void OnCurrentPageChanged()
        {
            // Get binding context of Sensor Details viewmodel
            var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
            if (this.CurrentPage.Title == "Sensor Statistics")
            {
                sensorDetailsViewModel?.SetSensorCommandType(SensorDetailsViewModel.SensorCommand.Statistics);
            }
            else if (this.CurrentPage.Title == "Sensor Limits")
            {
                sensorDetailsViewModel?.SetSensorCommandType(SensorDetailsViewModel.SensorCommand.Limits);
            }
            else
            {
                sensorDetailsViewModel?.SetSensorCommandType(SensorDetailsViewModel.SensorCommand.Plot);
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

        /// <summary>
        /// Handles the OnTextChanged event of the UpperCalibrationEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void UpperCalibrationEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _upperCalibrationEntryValue = e.NewTextValue;
            // Perform calculation
            CalculateScaleAndOffset();
        }

        /// <summary>
        /// Handles the OnTextChanged event of the UpperCalibrationTargetEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void UpperCalibrationTargetEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _upperCalibrationTargetEntryValue = e.NewTextValue;
            // Perform calculation
            CalculateScaleAndOffset();
        }

        /// <summary>
        /// Handles the OnTextChanged event of the LowerCalibrationEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void LowerCalibrationEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _lowerCalibrationEntryValue = e.NewTextValue;
            // Perform calculation
            CalculateScaleAndOffset();
        }

        /// <summary>
        /// Handles the OnTextChanged event of the LowerCalibrationTargetEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void LowerCalibrationTargetEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _lowerCalibrationTargetEntryalue = e.NewTextValue;
            // Perform calculation
            CalculateScaleAndOffset();
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
                // ignored (soak up division by zero error, etc)
            }
        }

        /// <summary>
        /// Handles the OnValueChanged event of the SegControl control.
        /// </summary>
        /// <remarks>
        /// Calls commands in viewmodel to load more plot data.
        /// </remarks>
        /// <param name="s">The source of the event.</param>
        /// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
        private void SegControl_OnValueChanged(object s, ValueChangedEventArgs e)
        {
            //SensorPlotChart.SuspendSeriesNotification();
            //MessagingCenter.Subscribe<SensorDetailsPage>(this, "Resume", (sender) => {
            //    SensorPlotChart.ResumeSeriesNotification();
            //    Application.Current.MainPage.DisplayAlert("CIMScan", "Resumed from Messaging Center", "Cancel");
            //});

            // Get binding context of Sensor Details viewmodel
            var sensorDetailsViewModel = (SensorDetailsViewModel)this.BindingContext;
            switch (e.NewValue)
            {
                case 0:
                    sensorDetailsViewModel.LoadMorePlotData1.Execute();
                    break;
                case 1:
                    sensorDetailsViewModel.LoadMorePlotData2.Execute();
                    break;
                case 2:
                    sensorDetailsViewModel.LoadMorePlotData3.Execute();
                    break;
                case 3:
                    sensorDetailsViewModel.LoadMorePlotData4.Execute();
                    break;
            }
        }
    }
}