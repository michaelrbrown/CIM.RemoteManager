using System;
using System.Threading.Tasks;
using CIM.RemoteManager.Core.Common;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.Helpers
{
    /// <summary>
    /// Class StationHelper.
    /// </summary>
    public class StationHelper
    {
        /// <summary>
        /// Handles the remote date time validation.
        /// </summary>
        /// <remarks>
        /// We validate the DA-12 station time and if it's before 2009 we know
        /// it's not valid, so we set a new time.
        /// We take a Unix timestamp as UTC and formats it into a station command to
        /// update the time.
        /// </remarks>
        /// <param name="txCharacteristic">Bluetooth service characteristic</param>
        /// <param name="remoteUnixDateTime">The remote Unix date time.</param>
        public async Task<bool> HandleRemoteDateTimeValidation(ICharacteristic txCharacteristic, int remoteUnixDateTime)
        {
            // Validate our station Unix time converted to windows time is less
            // than 2009.  If it is we know the station time needs to be set.
            if (remoteUnixDateTime.UnixTimeStampToDateTime().Year < CoreConstants.StationSettingLowestDateTimeYear)
            {
                // Get Unix timestamp "now" as UTC
                Int32 unixTimestampUtc = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Format DA-12 station Unix timestamp
                // T - is to set the station time,
                // 0 - is the data type
                // 00 - device index / unused
                // Next set of digits is Unix time UTC (converted to HEX)
                string remoteUnitTimestamp = "{T000" + unixTimestampUtc.ToString("X") + "}";

                // Send set Unix UTC time command to remote
                await txCharacteristic.WriteAsync(remoteUnitTimestamp.StrToByteArray()).ConfigureAwait(true);

                // Wait a couple seconds for remote to process
                await Task.Delay(3000).ConfigureAwait(true);

                // Let caller know we set station time
                return true;
            }

            // We didn't set station time
            return false;
        }

        /// <summary>
        /// Gets the alarm status.
        /// </summary>
        /// <param name="statusFlag">The status flag.</param>
        /// <returns>Alarm status <code>string</code>.</returns>
        public string GetAlarmStatus(int statusFlag)
        {
            bool successParse = int.TryParse(statusFlag.ToString(), out int alarmValue);
            if (successParse)
            {
                switch (alarmValue)
                {
                    case 0:
                        return "normal";
                    case 1:
                        return "warning";
                    case 2:
                        return "alarm";
                    case 3:
                        return "error";
                }

            }
            // Default
            return "normal";
        }
    }
}
