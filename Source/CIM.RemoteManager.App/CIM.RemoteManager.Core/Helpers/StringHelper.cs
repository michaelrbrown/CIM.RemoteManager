using System;
using System.IO;
using System.Text;

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

        /// <summary>
        /// Convert hex to ASCII text.
        /// </summary>
        /// <param name="hexValue">Hex string</param>
        /// <returns>ASCII <code>string</code></returns>
        public static string HexToAscii(this string hexValue)
        {
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < hexValue.Length; i += 2)
            {
                string str = hexValue.Substring(i, 2);
                output.Append((char)Convert.ToInt32(str, 16));
            }
            return output.ToString();
        }

        /// <summary>
        /// Convert a string to a byte array.
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns><code>Byte[]</code></returns>
        public static byte[] StrToByteArray(this string strValue)
        {
            Encoding encoding = Encoding.UTF8;
            // Return byte array with UTF9 encoding
            return encoding.GetBytes(strValue);
        }

        /// <summary>
        /// Convert a byte array to string.
        /// </summary>
        /// <param name="bytes">Bytes to convert to string.</param>
        /// <returns><code>string</code>.</returns>
        public static string ByteArrayToString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Byte to string converter.
        /// </summary>
        /// <param name="bytes"><code>byte[]</code> to convert.</param>
        /// <returns><code>string</code></returns>
        public static string BytesToStringConverted(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

    }
}
