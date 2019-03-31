using System;

namespace Domain
{
    public class MissingNameOfInsuredObjectException : Exception
    {
        public MissingNameOfInsuredObjectException()
        {
        }

        public MissingNameOfInsuredObjectException(string message) : base(message)
        {
        }
    }
}
