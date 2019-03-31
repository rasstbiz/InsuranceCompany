using System;

namespace Domain
{
    public class EffectivePolicyNotFoundException : Exception
    {
        public EffectivePolicyNotFoundException()
        {
        }

        public EffectivePolicyNotFoundException(string message) : base(message)
        {
        }
    }
}
