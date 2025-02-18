﻿using Foundation;
using HockeyApp.iOS;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using SegmentedControl.FormsPlugin.iOS;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.SfChart.XForms.iOS.Renderers;
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
            SlideOverKit.iOS.SlideOverKit.Init();

            // Make UIWindow they key window
            _window.MakeKeyAndVisible();

            // Setup color scheme
            UIColor accentColor = UIColor.FromRGB(1, 100, 157);
            UIColor secondaryAccentColor = UIColor.FromRGB(237, 239, 240);
            // UISlider
            UISlider.Appearance.TintColor = accentColor;
            UISlider.Appearance.ThumbTintColor = accentColor;
            // UISwitch
            UISwitch.Appearance.TintColor = accentColor;
            UISwitch.Appearance.OnTintColor = accentColor;
            // UITabBar
            UITabBar.Appearance.BarTintColor = secondaryAccentColor;
            UITabBar.Appearance.BackgroundColor = secondaryAccentColor;
            UITabBar.Appearance.TintColor = UIColor.FromRGB(1, 100, 157); ;
            UITabBar.Appearance.SelectedImageTintColor = UIColor.FromRGB(1, 100, 157);
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
