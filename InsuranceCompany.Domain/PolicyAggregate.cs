﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class PolicyAggregate
    {
        private readonly IPolicy policy;

        public PolicyAggregate(IPolicy policy)
        {
            this.policy = policy;
        }

        public void Create(string nameOfInsuredObject, InsurancePeriod insurancePeriod, IList<Risk> insuredRisks)
        {
            if (string.IsNullOrEmpty(nameOfInsuredObject))
            {
                throw new MissingNameOfInsuredObjectException("Missing Name of Insured Object");
            }

            var initialRiskSet = new InitialRiskSet(insuredRisks, insurancePeriod);
            policy.NameOfInsuredObject = nameOfInsuredObject;
            policy.ValidFrom = insurancePeriod.From;
            policy.ValidTill = insurancePeriod.Till;
            policy.InsuredRisks = insuredRisks;
            policy.Premium = initialRiskSet.CalculatePremium();
        }

        public void AddRisk(Risk risk, DateTime validFrom)
        {
            if (policy.InsuredRisks.Any(r => r.Name.ToLowerInvariant() == risk.Name.ToLowerInvariant()))
            {
                throw new ExistingRiskException("Risk with such name is already insured");
            }

            var insurancePeriod = new InsurancePeriod(validFrom, policy.ValidTill);
            var newRiskInsurancePeriod = new RiskInsurancePeriod(insurancePeriod, risk);

            policy.Premium += newRiskInsurancePeriod.CalculatePremium();
            policy.InsuredRisks.Add(risk);
        }

        public void RemoveRisk(Risk risk, DateTime validTill)
        {
            if (!policy.InsuredRisks.Any(r => r.Name.ToLowerInvariant() == risk.Name.ToLowerInvariant()))
            {
                throw new InvalidRiskException("Risk with such name is not insured");
            }

            var insurancePeriod = new InsurancePeriod(validTill.AddMonths(1), policy.ValidTill);
            var nonInsuredRiskPeriod = new RiskInsurancePeriod(insurancePeriod, risk);

            policy.Premium -= nonInsuredRiskPeriod.CalculatePremium();
            policy.InsuredRisks.Remove(risk);
        }
    }
}
