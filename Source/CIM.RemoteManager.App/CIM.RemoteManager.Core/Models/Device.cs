using System;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// Remote model
    /// </summary>
    public class Remote
    {
        /// <summary>
        /// Remote image
        /// </summary>
        public ImageSource RemoteImage { get; set; } = "";
        /// <summary>
        /// Remote name
        /// </summary>
        public string Name { get; set; } = "Fridge #1";
        /// <summary>
        /// Remote data
        /// </summary>
        private string _data;
        public string Data
        {
            get => this._data;
            set
            {
                if (_data != value)
                {
                    this._data = value;
                }
            }
        }
        public string Status { get; set; } = "Normal";
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string SerialNumber { get; set; } = "26512";
        public string SensorType { get; set; } = "Temp";

    }

}
