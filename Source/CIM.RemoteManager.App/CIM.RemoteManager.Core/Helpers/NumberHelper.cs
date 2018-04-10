using System;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class NumberHelper
    {
        /// <summary>
        /// Validates the number to be sure it doesn't return 32768.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Validated result</returns>
        public static double ValidateNumber(this double number)
        {
            if (Math.Abs(number - 32768) < 0.001)
            {
                return 0;
            }
            // Return result
            return number;
        }

        /// <summary>
        /// Safely converts a given type to another type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringValue">The string value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isHex">if set to <c>true</c> [is hexadecimal].</param>
        /// <returns><code>Generic Type</code></returns>
        public static T SafeConvert<T>(this string stringValue, T defaultValue, bool isHex = false)
        {
            if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;
            // Return converted value
            return (T)Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Safely convert hex values to int
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>Float</returns>
        public static int SafeHexToInt(this string hexValue)
        {
            if (String.IsNullOrWhiteSpace(hexValue)) return 0;
            // Return converted value
            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Safely convert hex values to double
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>double</returns>
        public static double SafeHexToDouble(this string hexValue)
        {
            if (String.IsNullOrWhiteSpace(hexValue)) return 0;
            // Return converted value
            hexValue = hexValue.Replace("x", string.Empty);
            int.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out int result);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Safely convert hex values to decimal
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>decimal</returns>
        public static decimal SafeHexToDecimal(this string hexValue)
        {
            if (String.IsNullOrWhiteSpace(hexValue)) return 0;
            // Return converted value
            hexValue = hexValue.Replace("x", string.Empty);
            int.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out int result);
            return Convert.ToDecimal(result);
        }

    }
}
