using System;

namespace Domain
{
    public class AddedRiskUnavailableException : Exception
    {
        public AddedRiskUnavailableException()
        {
        }

        public AddedRiskUnavailableException(string message) : base(message)
        {
        }
    }
}
