using System;

namespace Domain
{
    public class ExistingRiskException : Exception
    {
        public ExistingRiskException()
        {
        }

        public ExistingRiskException(string message) : base(message)
        {
        }
    }
}
