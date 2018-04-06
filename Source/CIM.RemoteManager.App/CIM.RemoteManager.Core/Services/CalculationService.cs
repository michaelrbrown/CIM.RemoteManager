namespace CIM.RemoteManager.Core.Services
{
    /// <summary>
    /// Class CalculationService.
    /// </summary>
    public class CalculationService
    {
        /// <summary>
        /// Average conversion.
        /// </summary>
        /// <param name="averageValue">The average value.</param>
        /// <param name="decimalLocation">The decimal location.</param>
        /// <returns>System.Double.</returns>
        public double AverageConversion(double averageValue, int decimalLocation)
        {
            return (averageValue * decimalLocation) / 100.0;
        }
    }
}
