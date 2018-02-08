using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class SensorTypeConverter
    {
        public static T SafeConvert<T>(this string stringValue, T defaultValue, bool isHex = false)
        {
            if (string.IsNullOrEmpty(stringValue)) return defaultValue;
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Safely convert hex values to int
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>Float</returns>
        public static int SafeHexToInt(this string hexValue)
        {
            if (String.IsNullOrEmpty(hexValue)) return 0;

            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Safely convert hex values to float
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>Float</returns>
        public static float SafeHexToFloat(this string hexValue)
        {
            if (String.IsNullOrEmpty(hexValue)) return 0;

            hexValue = hexValue.Replace("x", string.Empty);
            float.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out float result);
            return result;
        }

        /// <summary>
        /// Safely convert hex values to decimal
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>Float</returns>
        public static decimal SafeHexToDecimal(this string hexValue)
        {
            List<int> dec = new List<int> { 0 };   // decimal result

            foreach (char c in hexValue)
            {
                int carry = Convert.ToInt32(c.ToString(), 16);
                // initially holds decimal value of current hex digit;
                // subsequently holds carry-over for multiplication

                for (int i = 0; i < dec.Count; ++i)
                {
                    int val = dec[i] * 16 + carry;
                    dec[i] = val % 10;
                    carry = val / 10;
                }

                while (carry > 0)
                {
                    dec.Add(carry % 10);
                    carry /= 10;
                }
            }

            var chars = dec.Select(d => (char)('0' + d));
            var cArr = chars.Reverse().ToArray();

            
            try
            {
                return Convert.ToDecimal(cArr);
            }
            catch (OverflowException overflowException)
            {
                Application.Current.MainPage.DisplayAlert("OverflowException", overflowException.Message, "Ok");
            }
            catch (FormatException formatException)
            {
                Application.Current.MainPage.DisplayAlert("FormatException", formatException.Message, "Ok");
            }
            catch (ArgumentNullException argumentNullException)
            {
                Application.Current.MainPage.DisplayAlert("ArgumentNullException", argumentNullException.Message, "Ok");
            }


            return 0;
        }

        public static string HexToDecimal(string hex)
        {
            List<int> dec = new List<int> { 0 };   // decimal result

            foreach (char c in hex)
            {
                int carry = Convert.ToInt32(c.ToString(), 16);
                // initially holds decimal value of current hex digit;
                // subsequently holds carry-over for multiplication

                for (int i = 0; i < dec.Count; ++i)
                {
                    int val = dec[i] * 16 + carry;
                    dec[i] = val % 10;
                    carry = val / 10;
                }

                while (carry > 0)
                {
                    dec.Add(carry % 10);
                    carry /= 10;
                }
            }

            var chars = dec.Select(d => (char)('0' + d));
            var cArr = chars.Reverse().ToArray();
            return new string(cArr);
        }

    }
}
