using System;
using System.Linq;

namespace Domain
{
    public class InsuranceCompany
    {
        private readonly IPolicyRepository policyRepository;

        public InsuranceCompany(IPolicyRepository policyRepository)
        {
            this.policyRepository = policyRepository;
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            var policies = policyRepository.FindByNameOfInsuredObject(nameOfInsuredObject)
                .Where(p => p.ValidFrom <= effectiveDate)
                .Where(p => p.ValidTill >= effectiveDate);

            if (policies == null || !policies.Any())
            {
                throw new EffectivePolicyNotFoundException("Effective policy for requested arguments could not be found"); 
            }
            if (policies.Count() > 1)
            {
                throw new MultipleEffectivePoliciesFoundException("More than one effective policies found");
            }

            return policies.Single();
        }
    }
}
