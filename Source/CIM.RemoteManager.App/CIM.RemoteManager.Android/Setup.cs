using Acr.UserDialogs;
using Android.Content;
using CIM.RemoteManager.Core;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using Plugin.Settings;
using Plugin.Permissions;
using MvvmCross.Forms.Droid.Presenters;

namespace CIM.RemoteManager.Android
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new BleMvxApplication();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var presenter = new MvxFormsDroidPagePresenter();
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
            return presenter;
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            Mvx.RegisterSingleton(() => UserDialogs.Instance);
            Mvx.RegisterSingleton(() => CrossSettings.Current);
            Mvx.RegisterSingleton(() => CrossPermissions.Current);
        }
    }
}
