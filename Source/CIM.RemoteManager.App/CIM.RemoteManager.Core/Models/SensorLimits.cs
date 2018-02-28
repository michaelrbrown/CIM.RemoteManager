
namespace CIM.RemoteManager.Core.Models
{
    public class SensorLimits : BindableBase
    {
        private int _sensorIndex;
        public int SensorIndex
        {
            get => _sensorIndex;
            set => SetProperty(ref _sensorIndex, value);
        }

        /// <summary>
        /// Alarm status
        /// </summary>
        private int _alarmStatus;
        public int AlarmStatus
        {
            get => _alarmStatus;
            set => SetProperty(ref _alarmStatus, value);
        }

        /// <summary>
        /// Alarm being processed
        /// </summary>
        private int _alarmBeingProcessed;
        public int AlarmBeingProcessed
        {
            get => _alarmBeingProcessed;
            set => SetProperty(ref _alarmBeingProcessed, value);
        }

        /// <summary>
        /// Alarm delay
        /// </summary>
        private double _alarmDeley;
        public double AlarmDelay
        {
            get => _alarmDeley;
            set => SetProperty(ref _alarmDeley, value);
        }

        /// <summary>
        /// Low alarm limit
        /// </summary>
        private double _lowAlarmLimit;
        public double LowAlarmLimit
        {
            get => _lowAlarmLimit;
            set => SetProperty(ref _lowAlarmLimit, value);
        }
        
        /// <summary>
        /// Low warning limit
        /// </summary>
        private double _lowWarningLimit;
        public double LowWarningLimit
        {
            get => _lowWarningLimit;
            set => SetProperty(ref _lowWarningLimit, value);
        }

        /// <summary>
        /// High alarm limit
        /// </summary>
        private double _highAlarmLimit;
        public double HighAlarmLimit
        {
            get => _highAlarmLimit;
            set => SetProperty(ref _highAlarmLimit, value);
        }

        /// <summary>
        /// High warning limit
        /// </summary>
        private double _highWarningLimit;
        public double HighWarningLimit
        {
            get => _highWarningLimit;
            set => SetProperty(ref _highWarningLimit, value);
        }
    }
}