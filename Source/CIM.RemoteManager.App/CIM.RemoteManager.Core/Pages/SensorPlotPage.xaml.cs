using System;
using CIM.RemoteManager.Core.ViewModels;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorPlotPage : TabbedPage
    {
        public SensorPlotPage()
        {
            InitializeComponent();
            BindingContext = this; // Note that I added this line
            
            //SensorBusyIndicator.IsBusy = false;
        }
    }
}