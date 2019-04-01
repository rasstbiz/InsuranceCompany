namespace Domain
{
    public static class IPolicyExtensions
    {
        public static InsurancePeriod GetInsurancePeriod(this IPolicy policy)
        {
            return new InsurancePeriod(policy.ValidFrom, policy.ValidTill);
        }

        public static bool OverlapsWith(this IPolicy policy, InsurancePeriod period)
        {
            return policy.ValidFrom <= period.Till && period.From <= policy.ValidTill;
        }
    }
}
