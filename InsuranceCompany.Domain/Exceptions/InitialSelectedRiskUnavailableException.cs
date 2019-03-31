using System;

namespace Domain
{
    public class InitialSelectedRiskUnavailableException : Exception
    {
        public InitialSelectedRiskUnavailableException()
        {
        }

        public InitialSelectedRiskUnavailableException(string message) : base(message)
        {
        }
    }
}
