using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
