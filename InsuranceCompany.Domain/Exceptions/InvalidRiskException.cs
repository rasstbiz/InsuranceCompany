using System;

namespace Domain
{
    public class InvalidRiskException : Exception
    {
        public InvalidRiskException()
        {
        }

        public InvalidRiskException(string message) : base(message)
        {
        }
    }
}
