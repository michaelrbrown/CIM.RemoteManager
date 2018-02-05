using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor.
    /// </summary>
    public class Sensor : ISensor
    {
        public int Index { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string SensorType { get; set; }
        public float Scale { get; set; }
        public float Offset { get; set; }
        public int TimeStamp { get; set; }
        public float AverageValue { get; set; }
        public float CurrentValue { get; set; }
        public int DisplayConversionCode { get; set; }
        public int DecimalLocation { get; set; }
        public string StatisticsTotalCalcSettings { get; set; }
    }
}