using System;
using CIM.RemoteManager.Core.Helpers;
using Telerik.XamarinForms.Common.DataAnnotations;

namespace CIM.RemoteManager.Core.Models
{
    public class SensorStatistics : BindableBase
    {
        private int _sensorIndex;
        [Ignore]
        public int SensorIndex
        {
            get => _sensorIndex;
            set => SetProperty(ref _sensorIndex, value);
        }

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _averageValue;
        [DisplayOptions(Header = "Average")]
        public double AverageValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_averageValue.ToString(), out double averageValueResult))
                {
                    return averageValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _averageValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private double _sinceTimeStamp;
        [Ignore]
        public double TimeStamp
        {
            get => _sinceTimeStamp;
            set => SetProperty(ref _sinceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        [DisplayOptions(Header = "Since")]
        public DateTime? SinceDateTimeStamp => _sinceTimeStamp.UnixTimeStampToDateTime();


        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _minimumValue;
        [DisplayOptions(Header = "Minimum")]
        public double MinimumValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_minimumValue.ToString(), out double minimumValueResult))
                {
                    return minimumValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _minimumValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private double _minimumOccuranceTimeStamp;
        [Ignore]
        public double MinimumOccuranceTimeStamp
        {
            get => _minimumOccuranceTimeStamp;
            set => SetProperty(ref _minimumOccuranceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        [DisplayOptions(Header = "Occurance")]
        public DateTime? MinimumOccuranceDateTimeStamp => _minimumOccuranceTimeStamp.UnixTimeStampToDateTime();

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _maximumValue;
        [DisplayOptions(Header = "Maximum")]
        public double MaximumValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_maximumValue.ToString(), out double maximumValueResult))
                {
                    return maximumValueResult / 10;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _maximumValue, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        private double _maximumOccuranceTimeStamp;
        [Ignore]
        public double MaximumOccuranceTimeStamp
        {
            get => _maximumOccuranceTimeStamp;
            set => SetProperty(ref _maximumOccuranceTimeStamp, value);
        }

        /// <summary>
        /// Converting Unix to Windows DateTime
        /// </summary>
        [DisplayOptions(Header = "Occurance")]
        public DateTime? MaximumOccuranceDateTimeStamp => _maximumOccuranceTimeStamp.UnixTimeStampToDateTime();

        /// <summary>
        /// Average sensor value.
        /// Make sure we divide by 10 to convert to appropriate value.
        /// </summary>
        private double _varianceValue;
        [DisplayOptions(Header = "Variance")]
        public double VarianceValue
        {
            get
            {
                // Try to lookup hex to string
                if (double.TryParse(_maximumValue.ToString(), out double maximumValueResult) && (double.TryParse(_minimumValue.ToString(), out double minimumValueResult)))
                {
                    return maximumValueResult - minimumValueResult;
                }
                // Default
                return 0;
            }
            set => SetProperty(ref _varianceValue, value);
        }
    }
}