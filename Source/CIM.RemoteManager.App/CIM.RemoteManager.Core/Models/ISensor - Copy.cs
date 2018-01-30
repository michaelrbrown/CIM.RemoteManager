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
    public interface ISensor : IDisposable
    {
        float Scale { get; set; }
        float Offset { get; set; }
        Byte DispCode { get; set; }
        Byte Decimal { get; set; }
        sbyte Calc { get; set; }
        sbyte Alarms { get; set; }
        sbyte Delay { get; set; }
        float LowAlarm { get; set; }
        float LowWarn { get; set; }
        float HighWarn { get; set; }
        float HighAlarm { get; set; }
        float Max { get; set; }
        UInt32 MaxTime { get; set; }
        float Min { get; set; }
        UInt32 MinTime { get; set; }
        float Average { get; set; }
        float Special { get; set; }

        /// <summary>
        /// Gets the first characteristic with the Id <paramref name="id"/>. 
        /// </summary>
        /// <param name="id">The id of the searched characteristic.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. 
        /// The Result property will contain the characteristic with the specified <paramref name="id"/>.
        /// If the characteristic doesn't exist, the Result will be null.
        /// </returns>
        Task<Sensor> GetSensorDataAsync(Guid id);
    }
}