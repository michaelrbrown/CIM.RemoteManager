using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Forms.Core;
using MvvmCross.Platform;

namespace CIM.RemoteManager.Core
{
    public partial class BleMvxFormsApp : MvxFormsApplication
    {
        public BleMvxFormsApp()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();

            AppCenter.Start("ios=5a1074f6-8377-43b9-8690-fc377804ee13;" + "uwp={Your UWP App secret here};" +
                            "android=88ae5767-1223-44bc-a70e-ba476ef09724;",
                typeof(Analytics), typeof(Crashes));
            
            Mvx.Trace("App Start");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Mvx.Trace("App Resume");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Mvx.Trace("App Sleep");
        }
    }
}
