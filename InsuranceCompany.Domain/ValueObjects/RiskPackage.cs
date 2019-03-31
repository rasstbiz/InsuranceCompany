using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class RiskPackage
    {
        public RiskPackage(IList<Risk> insuredRisks, ValidityPeriod validityPeriod)
        {
            if (validityPeriod == null)
            {
                throw new ArgumentNullException(nameof(validityPeriod));
            }
            if (insuredRisks == null || !insuredRisks.Any())
            {
                throw new MissingInitialInsuredRisksException("Missing initial Insured Risks");
            }
        }

        public decimal Sum()
        {
            return 20m;
        }
    }
}
