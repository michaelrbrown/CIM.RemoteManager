﻿using System;
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

        /// <summary>
        /// Gets the first characteristic with the Id <paramref name="id"/>. 
        /// </summary>
        /// <param name="id">The id of the searched characteristic.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. 
        /// The Result property will contain the characteristic with the specified <paramref name="id"/>.
        /// If the characteristic doesn't exist, the Result will be null.
        /// </returns>
        Task<ISensor> GetSensorDataAsync(Guid id);
    }
}