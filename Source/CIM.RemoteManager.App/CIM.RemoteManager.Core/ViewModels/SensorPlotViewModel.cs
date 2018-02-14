using Acr.UserDialogs;
using MvvmCross.Core.ViewModels;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorPlotViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        

        public SensorPlotViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;
        }

        protected override async void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);
        }
    }
}
