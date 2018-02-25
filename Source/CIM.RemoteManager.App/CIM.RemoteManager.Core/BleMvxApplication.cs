using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace CIM.RemoteManager.Core
{
    public class BleMvxApplication : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.LazyConstructAndRegisterSingleton<IMvxMessenger, MvxMessengerHub>();
            RegisterAppStart<DeviceListViewModel>();
        }
    }
}
