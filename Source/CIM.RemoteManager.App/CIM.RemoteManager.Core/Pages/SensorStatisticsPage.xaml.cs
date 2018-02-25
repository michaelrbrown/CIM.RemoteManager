using Acr.UserDialogs;
using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorStatisticsPage : BasePage
    {
        public SensorStatisticsPage()
        {
            InitializeComponent();
            this.BindingContext = new SensorStatisticsViewModel(MvxMessenger, null, null, UserDialogs.Instance);
            //BindingContext = this; // Note that I added this line

            // Get instance of SensorPlotViewModel
           // var sensorStatisticsViewModel = (SensorStatisticsViewModel)this.BindingContext;

           // StatisticsDataForm.Source = sensorStatisticsViewModel.SensorStatisticsCollection;

            //SensorBusyIndicator.IsBusy = false;
        }
    }
}