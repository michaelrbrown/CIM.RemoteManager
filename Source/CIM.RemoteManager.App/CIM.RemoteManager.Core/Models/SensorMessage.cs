using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Plugins.Messenger;

namespace CIM.RemoteManager.Core.Models
{
    public class SensorMessage : MvxMessage
    {
        public readonly SensorDetailsViewModel.SensorCommand SensorCommand;

        public SensorMessage(object sender, SensorDetailsViewModel.SensorCommand sensorCommand) : base(sender)
        {
            this.SensorCommand = sensorCommand;
        }

    }
}
