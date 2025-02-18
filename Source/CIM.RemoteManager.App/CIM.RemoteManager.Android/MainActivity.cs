using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Droid.Presenters;
using MvvmCross.Platform;
using Plugin.Permissions;
using Syncfusion.SfBusyIndicator.XForms.Droid;
using Syncfusion.SfChart.XForms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CIM.RemoteManager.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            // Get instance of PCL
            var formsApp = new CIM.RemoteManager.Core.BleMvxFormsApp();
            // Load app
            LoadApplication(formsApp);

            // Init user dialogs
            UserDialogs.Init(this);
            // Init SyncFusion controls
            new SfChartRenderer();
            new SfBusyIndicatorRenderer();

            // Inject MvxViewPresenter
            var presenter = (MvxFormsDroidPagePresenter) Mvx.Resolve<IMvxViewPresenter>();
            presenter.FormsApplication = formsApp;
            // Now start the app
            Mvx.Resolve<IMvxAppStart>().Start();



            // HockeyApp
            CheckForUpdates();
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();

            //if (ContextCompat.CheckSelfPermission(this, "") != Permission.Granted)
            //{
            //    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("Permission Granted!!!");
            //}
        }

        /// <summary>
        /// Called when [resume].
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            CrashManager.Register(this, "7941bf481049476ca868b71fb4deadaa");
            MetricsManager.Register(Application, "7941bf481049476ca868b71fb4deadaa");
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        private void CheckForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this, "7941bf481049476ca868b71fb4deadaa");
        }

        /// <summary>
        /// Unregisters HockyApp update manager
        /// </summary>
        private void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }

        /// <summary>
        /// Called when [pause].
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            UnregisterManagers();
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterManagers();
        }

        /// <summary>
        /// Get our app's location permissions.
        /// Prompt's the user for permission.
        /// </summary>
        private async void GetLocationPermissions()
        {
            await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.LocationAlways);
        }

    }
}