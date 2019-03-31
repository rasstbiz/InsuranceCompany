namespace Domain
{
    public static class IPolicyExtensions
    {
        public static ValidityPeriod GetValidityPeriod(this IPolicy policy)
        {
            return new ValidityPeriod(policy.ValidFrom, policy.ValidTill);
        }
    }
}
