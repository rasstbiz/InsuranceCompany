using System;

namespace Domain
{
    public class RiskValidityPeriod
    {
        public string RiskName { get; }

        private readonly ValidityPeriod validityPeriod;
        private readonly Risk risk;

        public RiskValidityPeriod(ValidityPeriod validityPeriod, Risk risk)
        {
            if (validityPeriod == null)
            {
                throw new ArgumentNullException(nameof(validityPeriod));
            }

            this.validityPeriod = validityPeriod;
            this.risk = risk;

            RiskName = risk.Name;
        }

        public decimal CalculatePremium()
        {
            return (risk.YearlyPrice / 12) * validityPeriod.Months;
        }
    }
}
