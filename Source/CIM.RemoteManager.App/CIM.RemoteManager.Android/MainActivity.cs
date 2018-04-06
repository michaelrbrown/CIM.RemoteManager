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
using Syncfusion.SfBusyIndicator.XForms.Droid;
using Syncfusion.SfChart.XForms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CIM.RemoteManager.Android
{
    /// <summary>
    /// Class MainActivity.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.Android.FormsAppCompatActivity" />
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            UserDialogs.Init(this);
            Forms.Init(this, bundle);
            var formsApp = new CIM.RemoteManager.Core.BleMvxFormsApp();
            LoadApplication(formsApp);

            var presenter = (MvxFormsDroidPagePresenter) Mvx.Resolve<IMvxViewPresenter>();
            presenter.FormsApplication = formsApp;

            Mvx.Resolve<IMvxAppStart>().Start();

            // HockeyApp
            CheckForUpdates();

            // Init SyncFusion controls
            new SfChartRenderer();
            new SfBusyIndicatorRenderer();
        }

        /// <summary>
        /// Called when [resume].
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            CrashManager.Register(this, "7941bf481049476ca868b71fb4deadaa");
            // in your main activity OnCreate-method add:
            MetricsManager.Register(Application, "7941bf481049476ca868b71fb4deadaa");
        }

        private void CheckForUpdates()
        {
            // Remove this for store builds!
            UpdateManager.Register(this, "7941bf481049476ca868b71fb4deadaa");
        }

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

    }
}