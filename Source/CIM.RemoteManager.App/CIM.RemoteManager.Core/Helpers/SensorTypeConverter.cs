using System;

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
            if (String.IsNullOrEmpty(hexValue)) return 0;

            hexValue = hexValue.Replace("x", string.Empty);
            decimal.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out decimal result);
            return result;
        }

    }
}
