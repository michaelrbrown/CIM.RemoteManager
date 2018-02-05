using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class SensorTypeConverter
    {
        public static T SafeConvert<T>(this string s, T defaultValue)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;
            return (T)Convert.ChangeType(s, typeof(T));
        }
    }
}
