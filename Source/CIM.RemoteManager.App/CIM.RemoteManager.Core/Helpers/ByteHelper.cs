using System;
using System.Linq;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class ByteHelper
    {
        /// <summary>
        /// Convert text to bytes
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>byte array</returns>
        private static byte[] GetBytes(this string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
        }
    }
}
