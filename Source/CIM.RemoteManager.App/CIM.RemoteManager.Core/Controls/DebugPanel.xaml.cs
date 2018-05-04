
using SlideOverKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CIM.RemoteManager.Core.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DebugPanel : SlideMenuView
    {
        public DebugPanel()
        {
            InitializeComponent();

            // You must set HeightRequest in this case
            this.HeightRequest = 250;
            // You must set IsFullScreen in this case, 
            // otherwise you need to set WidthRequest, 
            // just like the QuickInnerMenu sample
            this.IsFullScreen = true;
            this.MenuOrientations = MenuOrientation.BottomToTop;

            // You must set BackgroundColor, 
            // and you cannot put another layout with background color cover the whole View
            // otherwise, it cannot be dragged on Android
            this.BackgroundColor = Color.Black;
            this.BackgroundViewColor = Color.Transparent;

            // In some small screen size devices, the menu cannot be full size layout.
            // In this case we need to set different size for Android.
            if (Device.RuntimePlatform == Device.Android)
                this.HeightRequest += 50;
        }

        public string DebugText { get; set; }

        private static BindableProperty DebugTextProperty = BindableProperty.Create(
            propertyName: "DebugText",
            returnType: typeof(string),
            declaringType: typeof(DebugPanel),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TextPropertyChanged);

        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (DebugPanel)bindable;
            control.DebugLabel.Text = newValue.ToString();
        }

    }
}