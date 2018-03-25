namespace CIM.RemoteManager.Core.Models
{
    public class MessageCounters : BindableBase
    {
        private int _totalOutgoingMessages;
        public int TotalOutgoingMessages
        {
            get => _totalOutgoingMessages;
            set => SetProperty(ref _totalOutgoingMessages, value);
        }

        private int _totalOutgoingRetries;
        public int TotalOutgoingRetries
        {
            get => _totalOutgoingRetries;
            set => SetProperty(ref _totalOutgoingRetries, value);
        }

        private int _totalOutgoingValues;
        public int TotalOutgoingValues
        {
            get => _totalOutgoingValues;
            set => SetProperty(ref _totalOutgoingValues, value);
        }

        private int _totalIncomingMessages;
        public int TotalIncomingMessages
        {
            get => _totalIncomingMessages;
            set => SetProperty(ref _totalIncomingMessages, value);
        }

        private int _totalIncomingErrors;
        public int TotalIncomingErrors
        {
            get => _totalIncomingErrors;
            set => SetProperty(ref _totalIncomingErrors, value);
        }

        /// <summary>
        /// Time in minutes
        /// </summary>
        private int _lastServerMessageReceived;
        public int LastServerMessageReceived
        {
            get => _lastServerMessageReceived;
            set => SetProperty(ref _lastServerMessageReceived, value);
        }

        /// <summary>
        /// Total number of sensors attached to remote
        /// </summary>
        private int _totalActiveSensors;
        public int TotalActiveSensors
        {
            get => _totalActiveSensors;
            set => SetProperty(ref _totalActiveSensors, value);
        }

        /// <summary>
        /// Number of records in history buffer (this value is divided by 256)
        /// </summary>
        private int _totalRecordsInHistoryBuffer;
        public int TotalRecordsInHistoryBuffer
        {
            get
            {
                // Try to lookup hex to string
                if (int.TryParse(_totalRecordsInHistoryBuffer.ToString(), out int totalRecordsInHistoryBufferResult))
                {
                    return totalRecordsInHistoryBufferResult / 256;
                }
                // Default
                return _totalRecordsInHistoryBuffer;
            }
            set => SetProperty(ref _totalRecordsInHistoryBuffer, value);
        }

        /// <summary>
        /// Present unix time
        /// </summary>
        private string _currentDateTime;
        public string CurrentDateTime
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }

    }
}
