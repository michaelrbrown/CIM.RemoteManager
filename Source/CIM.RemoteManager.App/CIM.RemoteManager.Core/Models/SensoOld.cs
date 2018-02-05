using System;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Models
{
    public partial class Sensor 
    {

        public Sensor(ImageSource sensorImage, string name, string value, string status, DateTime timeStamp, string serialNumber, string sensorType)
        {

            this.SensorImage = ImageSource.FromResource($"CIM.RemoteDisplay.Portable.Views.SampleData.{sensorImage}.png");
            this.Name = name;
            this.Value = value;
            this.Status = status;
            this.TimeStamp = timeStamp;
            this.SerialNumber = serialNumber;
            this.SensorType = sensorType;
        }

        public ImageSource SensorImage { get; set; } = "";
        public string Name { get; set; } = "Fridge #1";

        private string _value;
        public string Value
        {
            get => this._value;
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                }
            }
        }
        public string Status { get; set; } = "Normal";
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string SerialNumber { get; set; } = "26512";
        public string SensorType { get; set; } = "Temp";

    }

}
