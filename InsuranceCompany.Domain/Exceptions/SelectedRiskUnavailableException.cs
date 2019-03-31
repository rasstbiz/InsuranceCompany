using System;

namespace Domain
{
    public class SelectedRiskUnavailableException : Exception
    {
        public SelectedRiskUnavailableException()
        {
        }

        public SelectedRiskUnavailableException(string message) : base(message)
        {
        }
    }
}
