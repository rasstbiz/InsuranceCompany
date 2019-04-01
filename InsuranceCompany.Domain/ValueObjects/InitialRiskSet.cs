using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class InitialRiskSet
    {
        private readonly Dictionary<string, RiskInsurancePeriod> insuredRisks;

        public InitialRiskSet(IList<Risk> insuredRisks, InsurancePeriod insurancePeriod, params RiskInsurancePeriod[] additionalInsuredPeriods)
        {
            if (insurancePeriod == null)
            {
                throw new ArgumentNullException(nameof(insurancePeriod));
            }
            if (insuredRisks == null || !insuredRisks.Any())
            {
                throw new MissingInitialInsuredRisksException("Missing initial Insured Risks");
            }

            this.insuredRisks = insuredRisks.ToDictionary(
                r => r.Name, r => new RiskInsurancePeriod(insurancePeriod, r));

            foreach (var additionalInsuredPeriod in additionalInsuredPeriods)
            {
                this.insuredRisks.Add(additionalInsuredPeriod.RiskName, additionalInsuredPeriod);
            }
        }

        public decimal CalculatePremium()
        {
            return insuredRisks.Sum(r => r.Value.CalculatePremium());
        }
    }
}
