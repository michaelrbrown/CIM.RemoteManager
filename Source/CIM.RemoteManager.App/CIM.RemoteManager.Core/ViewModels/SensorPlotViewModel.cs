using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CIM.RemoteManager.Core.Models;
using MvvmCross.Core.ViewModels;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.ViewModels
{
    public class SensorPlotViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        public ISensor Sensor { get; private set; }

        //public string SensorValue => Sensor?.Value.ToHexString().Replace("-", " ");

        public SensorPlotViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;
        }

        protected override async void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            //SensorPlot = await GetSensorPlotAsync(parameters);
        }

    }
}
