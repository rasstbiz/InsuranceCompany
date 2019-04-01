using System;

namespace Domain
{
    public class InvalidInsurancePeriodException : Exception
    {
        public InvalidInsurancePeriodException()
        {
        }

        public InvalidInsurancePeriodException(string message) : base(message)
        {
        }
    }
}
