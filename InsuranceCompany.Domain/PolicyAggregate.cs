using System;
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

        public void Create(string nameOfInsuredObject, ValidityPeriod validityPeriod, IList<Risk> insuredRisks)
        {
            if (string.IsNullOrEmpty(nameOfInsuredObject))
            {
                throw new MissingNameOfInsuredObjectException("Missing Name of Insured Object");
            }

            var riskPackage = new RiskPackage(insuredRisks, validityPeriod);
            policy.NameOfInsuredObject = nameOfInsuredObject;
            policy.ValidFrom = validityPeriod.From;
            policy.ValidTill = validityPeriod.Till;
            policy.InsuredRisks = insuredRisks;
            policy.Premium = riskPackage.CalculatePremium();
        }

        public void AddRisk(Risk risk, DateTime validFrom)
        {
            if (policy.InsuredRisks.Any(r => r.Name.ToLowerInvariant() == risk.Name.ToLowerInvariant()))
            {
                throw new ExistingRiskException("Risk with such name is already insured");
            }

            var validityPeriod = new ValidityPeriod(validFrom, policy.ValidTill);
            var newRiskValidityPeriod = new RiskValidityPeriod(validityPeriod, risk);

            policy.Premium += newRiskValidityPeriod.CalculatePremium();
            policy.InsuredRisks.Add(risk);
        }

        public void RemoveRisk(Risk risk, DateTime validTill)
        {
            if (!policy.InsuredRisks.Any(r => r.Name.ToLowerInvariant() == risk.Name.ToLowerInvariant()))
            {
                throw new InvalidRiskException("Risk with such name is not insured");
            }

            var validityPeriod = new ValidityPeriod(validTill.AddMonths(1), policy.ValidTill);
            var nonInsuredRiskPeriod = new RiskValidityPeriod(validityPeriod, risk);

            policy.Premium -= nonInsuredRiskPeriod.CalculatePremium();
            policy.InsuredRisks.Remove(risk);
        }
    }
}
