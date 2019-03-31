using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class InsuranceCompany
    {
        public IEnumerable<Risk> AvailableRisks { get; set; }

        private readonly IPolicyRepository policyRepository;

        public InsuranceCompany(IPolicyRepository policyRepository)
        {
            this.policyRepository = policyRepository;
        }

        public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths, IList<Risk> selectedRisks)
        {
            var validityPeriod = new ValidityPeriod(validFrom, validMonths);
            var effectivePolicies = GetPolicies(nameOfInsuredObject, validFrom);
            if (effectivePolicies.Any())
            {
                throw new ExistingEffectivePolicyException("Effective policy found that conflicts with requested validity period");
            }
            if (selectedRisks.Except(AvailableRisks).Any())
            {
                throw new InitialSelectedRiskUnavailableException("Some of the selected risks are not avaiable for selling");
            }

            var policy = new Policy();
            var policyAggregate = new PolicyAggregate(policy);
            policyAggregate.Create(nameOfInsuredObject, validityPeriod, selectedRisks);
            return policy;
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            var effectivePolicies = GetPolicies(nameOfInsuredObject, effectiveDate);
            if (effectivePolicies == null || !effectivePolicies.Any())
            {
                throw new EffectivePolicyNotFoundException("Effective policy for requested arguments could not be found"); 
            }
            if (effectivePolicies.Count() > 1)
            {
                throw new MultipleEffectivePoliciesFoundException("More than one effective policies found");
            }

            return effectivePolicies.Single();
        }

        private IEnumerable<IPolicy> GetPolicies(string nameOfInsuredObject, DateTime effectiveDate)
        {
            return policyRepository.FindByNameOfInsuredObject(nameOfInsuredObject)
                .Where(p => p.ValidFrom <= effectiveDate)
                .Where(p => p.ValidTill >= effectiveDate);
        }
    }
}
