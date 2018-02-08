using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}