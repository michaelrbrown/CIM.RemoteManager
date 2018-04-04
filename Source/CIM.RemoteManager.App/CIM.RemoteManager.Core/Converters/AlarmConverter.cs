using System;
using System.Globalization;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
    /// <summary>
    /// Class AlarmStatusConverter.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class AlarmStatusConverter : IValueConverter
    {
        /// <summary>
        /// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <returns>
        /// Returns the sensor alarm level.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool successParse = int.TryParse(value.ToString(), out int alarmValue);
            if (successParse)
            {
                switch (alarmValue)
                {
                    case 0:
                        return "normal";
                    case 1:
                        return "warning";
                    case 2:
                        return "alarm";
                    case 3:
                        return "error";
                }

            }
            // Default
            return "normal";
        }

        /// <summary>
        /// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts an int alarm value into a hex color code.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class AlarmStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool successParse = int.TryParse(value.ToString(), out int alarmValue);
            if (successParse)
            {
                switch (alarmValue)
                {
                    case 0: // Normal (green)
                        return Color.FromHex("#03BB03");
                    case 1: // Warning (yellow)
                        return Color.FromHex("#CDAC00");
                    case 2: // Alarm (red)
                        return Color.FromHex("#9A1C1F");
                    case 3: // Error (blue)
                        return Color.FromHex("#2758A7");
                }

            }
            // Default - Normal (green)
            return Color.FromHex("#03BB03");
        }

        /// <summary>
        /// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to which to convert the value.</param>
        /// <param name="parameter">A parameter to use during the conversion.</param>
        /// <param name="culture">The culture to use during the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}