using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class InsuranceCompany : IInsuranceCompany
    {
        public string Name { get; }

        public IList<Risk> AvailableRisks { get; set; }

        private readonly IPolicyRepository policyRepository;

        public InsuranceCompany(string name, IPolicyRepository policyRepository)
        {
            this.Name = name;
            this.policyRepository = policyRepository;
        }

        public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths, IList<Risk> selectedRisks)
        {
            var insurancePeriod = new NewInsurancePeriod(validFrom, validMonths);
            var effectivePolicies = GetEffectivePolicies(nameOfInsuredObject, validFrom);
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
            policyAggregate.Create(nameOfInsuredObject, insurancePeriod, selectedRisks);
            policyRepository.Add(policy);
            return policy;
        }

        public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom, DateTime effectiveDate)
        {
            if (!AvailableRisks.Contains(risk))
            {
                throw new AddedRiskUnavailableException("Added risk is not avaiable for selling");
            }

            var policy = GetInternalPolicy(nameOfInsuredObject, effectiveDate);
            var policyAggregate = new PolicyAggregate(policy);
            policyAggregate.AddRisk(risk, validFrom);
            policyRepository.Update(policy);
        }

        public void RemoveRisk(string nameOfInsuredObject, Risk risk, DateTime validTill, DateTime effectiveDate)
        {
            var policy = GetInternalPolicy(nameOfInsuredObject, effectiveDate);
            var policyAggregate = new PolicyAggregate(policy);
            policyAggregate.RemoveRisk(risk, validTill);
            policyRepository.Update(policy);
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            return GetInternalPolicy(nameOfInsuredObject, effectiveDate);
        }

        private Policy GetInternalPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            var effectivePolicies = GetEffectivePolicies(nameOfInsuredObject, effectiveDate);
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

        private IEnumerable<Policy> GetEffectivePolicies(string nameOfInsuredObject, DateTime effectiveDate)
        {
            return policyRepository.FindByNameOfInsuredObject(nameOfInsuredObject)
                .Where(p => p.ValidFrom <= effectiveDate)
                .Where(p => p.ValidTill >= effectiveDate);
        }
    }
}
