using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorStatisticsPage 
    {
        public SensorStatisticsPage()
        {
            InitializeComponent();
            //BindingContext = this; // Note that I added this line

            // Get instance of SensorPlotViewModel
            var sensorStatisticsViewModel = (SensorStatisticsViewModel)this.BindingContext;

            StatisticsDataForm.Source = sensorStatisticsViewModel.SensorStatisticsCollection;

            //SensorBusyIndicator.IsBusy = false;
        }
    }
}