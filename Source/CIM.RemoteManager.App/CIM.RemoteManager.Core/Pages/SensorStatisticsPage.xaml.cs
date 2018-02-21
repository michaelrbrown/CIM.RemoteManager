using Xamarin.Forms;
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
            
            //SensorBusyIndicator.IsBusy = false;
        }
    }
}