using Foundation;
using HockeyApp.iOS;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using SegmentedControl.FormsPlugin.iOS;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.XForms.iOS.PopupLayout;
using UIKit;

namespace CIM.RemoteManager.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        UIWindow _window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new Setup(this, _window);
            setup.Initialize();

            // Init application
            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            // HockeyApp
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure("7bbf2e7de05b4379a72711e0ba3c1e4a");
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation(); // This line is obsolete in crash only builds

            // Init SyncFusion controls
            new SfChartRenderer();
            new SfBusyIndicatorRenderer();
            SfListViewRenderer.Init();
            SegmentedControlRenderer.Init();
            SfPopupLayoutRenderer.Init();

            // Make UIWindow they key window
            _window.MakeKeyAndVisible();

            // Setup color scheme
            UIColor accentColor = UIColor.FromRGB(1, 100, 157);
            // UISlider
            UISlider.Appearance.TintColor = accentColor;
            UISlider.Appearance.ThumbTintColor = accentColor;
            // UISwitch
            UISwitch.Appearance.TintColor = accentColor;
            UISwitch.Appearance.OnTintColor = accentColor;
            // UITabBar
            UITabBar.Appearance.BackgroundImage = new UIImage();
            UITabBar.Appearance.BarTintColor = accentColor;
            UITabBar.Appearance.BackgroundColor = accentColor;
            UITabBar.Appearance.TintColor = UIColor.FromRGB(220, 225, 227);
            UITabBar.Appearance.SelectedImageTintColor = UIColor.White;
            // UINavigationBar
            UINavigationBar.Appearance.BarTintColor = accentColor;
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, TextShadowColor = UIColor.White});
            // UIToolbar
            UIToolbar.Appearance.BackgroundColor = accentColor;
            UIToolbar.Appearance.TintColor = UIColor.White;

            return true;
        }
    }
}
