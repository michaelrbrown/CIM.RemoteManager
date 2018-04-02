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
        public static int ValidateNumber(this double number)
        {
            if (int.TryParse(number.ToString(), out int result))
            {
                if (result == 32768)
                {
                    return 0;
                }
            }
            // Return result
            return result;
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
            if (string.IsNullOrEmpty(stringValue)) return defaultValue;
            return (T)Convert.ChangeType(stringValue, typeof(T));
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
        /// Safely convert hex values to double
        /// </summary>
        /// <param name="hexValue">Hex value to convert</param>
        /// <returns>double</returns>
        public static double SafeHexToDouble(this string hexValue)
        {
            if (String.IsNullOrEmpty(hexValue)) return 0;

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
            if (String.IsNullOrEmpty(hexValue)) return 0;

            hexValue = hexValue.Replace("x", string.Empty);
            int.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out int result);
            return Convert.ToDecimal(result);
        }

    }
}
