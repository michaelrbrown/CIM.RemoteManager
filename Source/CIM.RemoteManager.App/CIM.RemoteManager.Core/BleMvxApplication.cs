using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Core.ViewModels;

namespace CIM.RemoteManager.Core
{
    public class BleMvxApplication : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<DeviceListViewModel>();
        }
    }
}
