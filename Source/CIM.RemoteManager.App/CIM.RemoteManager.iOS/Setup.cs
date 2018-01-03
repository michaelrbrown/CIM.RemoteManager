using Acr.UserDialogs;
using CIM.RemoteManager.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS.Presenters;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using Plugin.Permissions;
using Plugin.Settings;
using UIKit;
using Xamarin.Forms;

namespace CIM.RemoteManager.iOS
{
    public class Setup : MvxIosSetup
    {
        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
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

        protected override IMvxIosViewPresenter CreatePresenter()
        {
            Forms.Init();
            var xamarinFormsApp = new CIM.RemoteManager.Core.BleMvxFormsApp();
            return new MvxFormsIosPagePresenter(Window, xamarinFormsApp);
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
