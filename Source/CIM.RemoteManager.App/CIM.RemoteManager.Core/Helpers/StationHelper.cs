using System;
using Plugin.BLE.Abstractions.Contracts;

namespace CIM.RemoteManager.Core.Helpers
{
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
        public async void HandleRemoteDateTimeValidation(ICharacteristic txCharacteristic, double remoteUnixDateTime)
        {
            // Validate our station Unix time converted to windows time is less 
            // than 2009.  If it is we know the station time needs to be set.
            if (remoteUnixDateTime.UnixTimeStampToDateTime().Year < 2009)
            {
                // Get Unix timestamp "now" as UTC
                Int32 unixTimestampUtc = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                // Format DA-12 station Unix timestamp
                // T - is to set the station time,
                // 0 - is the data type
                // 00 - device index / unused
                // Next set of digits is Unix time UTC
                string remoteUnitTimestamp = "{T000" + unixTimestampUtc + "}";

                // Send set Unix UTC time command to remote
                await txCharacteristic.WriteAsync(remoteUnitTimestamp.StrToByteArray()).ConfigureAwait(true);
            }
        }
    }
}
