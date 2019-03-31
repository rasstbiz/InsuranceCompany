using System;

namespace Domain
{
    public class MultipleEffectivePoliciesFoundException : Exception
    {
        public MultipleEffectivePoliciesFoundException()
        {
        }

        public MultipleEffectivePoliciesFoundException(string message) : base(message)
        {
        }
    }
}
