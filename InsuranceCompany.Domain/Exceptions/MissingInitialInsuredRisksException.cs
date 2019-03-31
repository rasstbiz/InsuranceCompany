using System;

namespace Domain
{
    public class MissingInitialInsuredRisksException : Exception
    {
        public MissingInitialInsuredRisksException()
        {
        }

        public MissingInitialInsuredRisksException(string message) : base(message)
        {
        }
    }
}
