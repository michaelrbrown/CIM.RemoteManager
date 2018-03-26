using System;
using CIM.RemoteManager.Core.Helpers;

namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor plot data
    /// </summary>
    public class SensorPlot : BindableBase
    {
        private int _points;
        public int Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        private double _unixTimeStamp;
        public double UnixTimeStamp
        {
            get => _unixTimeStamp;
            set => SetProperty(ref _unixTimeStamp, value);
        }

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        public DateTime? DateTimeStamp => _unixTimeStamp.UnixTimeStampToDateTime();

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }
    }
}