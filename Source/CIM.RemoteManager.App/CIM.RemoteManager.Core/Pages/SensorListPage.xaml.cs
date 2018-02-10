using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage : ContentPage
    {
        public SensorListPage()
        {
            InitializeComponent();

            BindingContext = this; // Note that I added this line
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //hrow new System.NotImplementedException();
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        private void ListView_OnStats(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
   
        private void ListView_OnSettings(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}