using System;
using System.Collections.Generic;

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
            var validityPeriodForNewRisk = new ValidityPeriod(validFrom, policy.ValidTill);
            var newInsuredPeriod = new InsuredPeriod(validityPeriodForNewRisk, risk);

            policy.Premium += newInsuredPeriod.CalculatePremium();
            policy.InsuredRisks.Add(risk);
        }
    }
}
