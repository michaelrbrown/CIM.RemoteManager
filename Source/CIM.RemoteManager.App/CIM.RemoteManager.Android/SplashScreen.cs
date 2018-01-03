using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Droid.Views;
using Xamarin.Forms;

namespace CIM.RemoteManager.Android
{
    [Activity(MainLauncher = true
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen
        : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }

        private bool _isInitializationComplete;
        public override void InitializationComplete()
        {
            if (!_isInitializationComplete)
            {
                _isInitializationComplete = true;
                StartActivity(typeof(MainActivity));
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            Forms.Init(this, bundle);
            Forms.ViewInitialized += (object sender, ViewInitializedEventArgs e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.View.StyleId))
                {
                    e.NativeView.ContentDescription = e.View.StyleId;
                }
            };

            base.OnCreate(bundle);
        }
    }
}
