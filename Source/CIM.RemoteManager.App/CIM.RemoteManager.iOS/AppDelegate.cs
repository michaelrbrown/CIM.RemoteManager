using Foundation;
using HockeyApp.iOS;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
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

            // Init Syncfusion controls
            new SfBusyIndicatorRenderer();
            SfListViewRenderer.Init();

            // Make UIWindow they key window
            _window.MakeKeyAndVisible();

            // Setup color scheme
            UIColor accentColor = UIColor.FromRGB(0, 151, 223);
            // UISlider
            UISlider.Appearance.TintColor = accentColor;
            UISlider.Appearance.ThumbTintColor = accentColor;
            // UISwitch
            UISwitch.Appearance.TintColor = accentColor;
            UISwitch.Appearance.OnTintColor = accentColor;
            // UITabBar
            UITabBar.Appearance.TintColor = accentColor;
            UITabBar.Appearance.SelectedImageTintColor = accentColor;
            // UINavigationBar
            UINavigationBar.Appearance.BarTintColor = accentColor;
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });
            // UIToolbar
            UIToolbar.Appearance.BackgroundColor = accentColor;
            UIToolbar.Appearance.TintColor = UIColor.White;

            return true;
        }
    }
}
