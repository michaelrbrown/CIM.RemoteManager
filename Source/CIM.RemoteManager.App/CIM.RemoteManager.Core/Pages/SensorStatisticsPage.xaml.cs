using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Platform;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorStatisticsPage : BasePage
    {
        public SensorStatisticsPage()
        {
            InitializeComponent();
            Mvx.IocConstruct<SensorStatisticsViewModel>();
            
            //var sensorStatisticsViewModel = Mvx.Resolve<SensorStatisticsViewModel>();

            this.BindingContext = this;
            //BindingContext = this; // Note that I added this line

            // Get instance of SensorPlotViewModel
           // var sensorStatisticsViewModel = (SensorStatisticsViewModel)this.BindingContext;

           // StatisticsDataForm.Source = sensorStatisticsViewModel.SensorStatisticsCollection;

            //SensorBusyIndicator.IsBusy = false;
        }

        /// <summary>
        /// Handle toggling of sensor updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorUpdatesSwitchToggled(object sender, ToggledEventArgs e)
        {
            // Get instance of SensorStatisticsViewModel
            var sensorStatisticsViewModel = (SensorStatisticsViewModel)this.BindingContext;
            // Toggle sensor updates
            sensorStatisticsViewModel.ToggleUpdatesCommand.Execute(null);
        }

    }
}