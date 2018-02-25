using MvvmCross.Plugins.Messenger;

namespace CIM.RemoteManager.Core.Models
{
    public class SensorMessage : MvxMessage
    {
        public SensorMessage(object sender, Sensor sensor) : base(sender)
        {
            this.Sensor = sensor;
        }

        public Sensor Sensor
        {
            get;
            private set;
        }

    }
}
