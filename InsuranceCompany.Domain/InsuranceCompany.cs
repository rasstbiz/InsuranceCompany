﻿using System;
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
            var validityPeriod = new NewValidityPeriod(validFrom, validMonths);
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
            policyAggregate.Create(nameOfInsuredObject, validityPeriod, selectedRisks);
            return policy;
        }

        public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom, DateTime effectiveDate)
        {
            if (!AvailableRisks.Contains(risk))
            {
                throw new AddedRiskUnavailableException("Added risk is not avaiable for selling");
            }

            var policy = GetPolicy(nameOfInsuredObject, effectiveDate);
            var policyAggregate = new PolicyAggregate(policy);
            policyAggregate.AddRisk(risk, validFrom);
        }

        public void RemoveRisk(string nameOfInsuredObject, Risk risk, DateTime validTill, DateTime effectiveDate)
        {
            var policy = GetPolicy(nameOfInsuredObject, effectiveDate);
            var policyAggregate = new PolicyAggregate(policy);
            policyAggregate.RemoveRisk(risk, validTill);
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
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

        private IEnumerable<IPolicy> GetEffectivePolicies(string nameOfInsuredObject, DateTime effectiveDate)
        {
            return policyRepository.FindByNameOfInsuredObject(nameOfInsuredObject)
                .Where(p => p.ValidFrom <= effectiveDate)
                .Where(p => p.ValidTill >= effectiveDate);
        }
    }
}
