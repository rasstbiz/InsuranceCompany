namespace Domain
{
    public static class IPolicyExtensions
    {
        public static InsurancePeriod GetInsurancePeriod(this IPolicy policy)
        {
            return new InsurancePeriod(policy.ValidFrom, policy.ValidTill);
        }
    }
}
