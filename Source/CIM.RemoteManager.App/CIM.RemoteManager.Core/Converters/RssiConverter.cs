using System;
using System.Globalization;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
    /// <summary>
    /// Class RssiConverter.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class RssiConverter : IValueConverter
    {
        const int MinRssi = -100;
        const int MaxRssi = -55;

        /// <summary>
        /// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <returns>
        /// RSSI image showing signal strength.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rssiVal = "0";
            if (value != null)
            {
                rssiVal = value.ToString();
            }

            // Show RSSI image by signal strength
            bool successParse = int.TryParse(rssiVal, out int rssiValue);
            if (successParse)
            {
                // Get real RSSI by calculation
                //int realRssiValue = CalculateSignalLevel(rssiValue, 5);
                int realRssiValue = rssiValue;
                if (rssiValue > -55)
                {
                    return "rssi_5.png";
                }
                else if (rssiValue > -66 && rssiValue < -56)
                {
                    return "rssi_4.png";
                }
                else if (rssiValue > -77 && rssiValue < -67)
                {
                    return "rssi_3.png";
                }
                else if (rssiValue > -78 && rssiValue < -88)
                {
                    return "rssi_2.png";
                }
            }

            return "rssi_1.png";
        }

        /// <summary>
        /// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <returns>To be added.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>To be added.</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the signal level.
        /// </summary>
        /// <param name="rssi">The rssi.</param>
        /// <param name="numLevels">The number levels.</param>
        /// <returns><code>int</code></returns>
        public static int CalculateSignalLevel(int rssi, int numLevels)
        {
            if (rssi <= MinRssi)
            {
                return 0;
            }
            else if (rssi >= MaxRssi)
            {
                return numLevels - 1;
            }
            else
            {
                float inputRange = (MaxRssi - MinRssi);
                float outputRange = (numLevels - 1);
                return (int) ((float) (rssi - MinRssi) * outputRange / inputRange);
            }
        }

    }
}