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
        public string Name { get; set; } = "DA-12";
        public string Status { get; set; } = "Normal";
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string SerialNumber { get; set; } = "";
    }

}
