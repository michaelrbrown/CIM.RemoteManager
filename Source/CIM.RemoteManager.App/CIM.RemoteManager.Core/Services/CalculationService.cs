namespace CIM.RemoteManager.Core.Services
{
    public class CalculationService
    {
        public double AverageConversion(double averageValue, int decimalLocation)
        {
            return (averageValue * decimalLocation) / 100.0;
        }
    }
}
