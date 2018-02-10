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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CIM.RemoteManager.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity
        : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            UserDialogs.Init(this);
            // FFImageloading init
            //CachedImageRenderer.Init(enableFastRenderer: true);
            Forms.Init(this, bundle);
            var formsApp = new CIM.RemoteManager.Core.BleMvxFormsApp();
            LoadApplication(formsApp);

            var presenter = (MvxFormsDroidPagePresenter) Mvx.Resolve<IMvxViewPresenter>();
            presenter.FormsApplication = formsApp;

            Mvx.Resolve<IMvxAppStart>().Start();

            CheckForUpdates();
        }

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

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterManagers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterManagers();
        }

    }
}