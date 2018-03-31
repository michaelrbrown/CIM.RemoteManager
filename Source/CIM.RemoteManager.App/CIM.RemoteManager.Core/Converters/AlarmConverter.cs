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
                        return Color.FromHex("#639777");
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