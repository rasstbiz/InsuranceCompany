using System;

namespace Domain
{
    public class InvalidRiskRemovePeriodException : Exception
    {
        public InvalidRiskRemovePeriodException()
        {
        }

        public InvalidRiskRemovePeriodException(string message) : base(message)
        {
        }
    }
}
