using System;
using System.Globalization;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
    public class AlarmStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool successParse = int.TryParse(value.ToString(), out int alarmValue);
            if (successParse)
            {
                switch (alarmValue)
                {
                    case 0:
                        return "Status: normal";
                    case 1:
                        return "Status: warning";
                    case 2:
                        return "Status: alarm";
                    case 3:
                        return "Status: error";
                }

            }
            // Default
            return "Status: normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}