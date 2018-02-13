using System;
using System.Globalization;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
    public class RssiConverter : IValueConverter
    {
        const int MinRssi = -100;
        const int MaxRssi = -55;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rssiVal = "0";
            if (value != null)
            {
                rssiVal = value.ToString();
            }
                
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

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