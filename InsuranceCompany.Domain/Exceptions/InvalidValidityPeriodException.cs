using System;

namespace Domain
{
    public class InvalidValidityPeriodException : Exception
    {
        public InvalidValidityPeriodException()
        {
        }

        public InvalidValidityPeriodException(string message) : base(message)
        {
        }
    }
}
