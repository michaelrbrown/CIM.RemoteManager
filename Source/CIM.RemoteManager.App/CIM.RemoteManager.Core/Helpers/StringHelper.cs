using System;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Gets a string up to a character is found.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="stopAt">The stop at.</param>
        /// <returns>String of characters up to stop character.</returns>
        public static string GetUntilOrEmpty(this string text, string stopAt = "}")
        {
            if (!String.IsNullOrEmpty(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }
            // Default
            return String.Empty;
        }

        /// <summary>
        /// Formats the sensor index padding with leading zero's to a total width
        /// of 3 chars.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor index padded to two digit string.</returns>
        public static string GetSensorIndexFormatted(this string sensorIndex)
        {
            if (!String.IsNullOrEmpty(sensorIndex))
            {
                return sensorIndex.PadLeft(2, '0');
            }
            // Default
            return "00";
        }

        /// <summary>
        /// Formats the sensor index as an integer.
        /// </summary>
        /// <param name="sensorIndex">Index of the sensor.</param>
        /// <returns>Sensor index as integer.</returns>
        public static int GetSensorIndexAsInt(this string sensorIndex)
        {
            if (!String.IsNullOrEmpty(sensorIndex))
            {
                return Convert.ToInt32(sensorIndex);
            }
            // Default
            return 0;
        }

    }
}
