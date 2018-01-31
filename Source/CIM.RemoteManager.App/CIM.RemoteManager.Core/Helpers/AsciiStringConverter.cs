using System;
using System.Text;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class AsciiStringConverter
    {
        /// <summary>
        /// Convert hex to ascii text
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
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
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(this string strValue)
        {
            Encoding encoding = Encoding.UTF8; //or below line
            //System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
            return encoding.GetBytes(strValue);
        }

        /// <summary>
        /// Convert a byte array to string
        /// </summary>
        /// <param name="strValue" />
        /// <param name="bytes">Bytes to convert to string</param>
        /// <returns></returns>
        public static string ByteArrayToString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes);
        }

    }
}
