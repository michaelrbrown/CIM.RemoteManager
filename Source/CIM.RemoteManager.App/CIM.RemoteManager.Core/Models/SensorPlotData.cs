namespace CIM.RemoteManager.Core.Models
{
    /// <summary>
    /// A DA-12 sensor plot data
    /// </summary>
    public class SensorPlotData : BindableBase
    {
       
        private int _points;
        public int Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }
        
        private double _timeStamp;
        public double TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }
        
        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }
        

    }
}