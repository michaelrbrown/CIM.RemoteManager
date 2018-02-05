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
    public interface ISensor
    {
        int Index { get; set; }
        string SerialNumber { get; set; }
        string Name { get; set; }
        string SensorType { get; set; }
        float Scale { get; set; }
        float Offset { get; set; }
        int TimeStamp { get; set; }
        float AverageValue { get; set; }
        float CurrentValue { get; set; }
        int DisplayConversionCode { get; set; }
        int DecimalLocation { get; set; }
        string StatisticsTotalCalcSettings { get; set; }
    }
}