using System;

namespace Domain
{
    public class ExistingEffectivePolicyException : Exception
    {
        public ExistingEffectivePolicyException()
        {
        }

        public ExistingEffectivePolicyException(string message) : base(message)
        {
        }
    }
}
