using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class RiskPackage
    {
        private readonly Dictionary<string, InsuredPeriod> insuredRisks;

        public RiskPackage(IList<Risk> insuredRisks, ValidityPeriod validityPeriod, params InsuredPeriod[] additionalInsuredPeriods)
        {
            if (validityPeriod == null)
            {
                throw new ArgumentNullException(nameof(validityPeriod));
            }
            if (insuredRisks == null || !insuredRisks.Any())
            {
                throw new MissingInitialInsuredRisksException("Missing initial Insured Risks");
            }

            this.insuredRisks = insuredRisks.ToDictionary(
                r => r.Name, r => new InsuredPeriod(validityPeriod, r));

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
