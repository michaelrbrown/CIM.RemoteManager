using System;
using System.Globalization;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
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
                    case 0:
                        return Color.FromHex("#03BB03");
                    case 1:
                        return Color.FromHex("#CDAC00");
                    case 2:
                        return Color.FromHex("#9A1C1F");
                    case 3:
                        return Color.FromHex("#2758A7");
                }

            }
            // Default
            return Color.ForestGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}