﻿using System;

namespace Domain
{
    public class RiskInsurancePeriod
    {
        public string RiskName { get; }

        private readonly InsurancePeriod insurancePeriod;
        private readonly Risk risk;

        public RiskInsurancePeriod(InsurancePeriod insurancePeriod, Risk risk)
        {
            if (insurancePeriod == null)
            {
                throw new ArgumentNullException(nameof(insurancePeriod));
            }

            this.insurancePeriod = insurancePeriod;
            this.risk = risk;

            RiskName = risk.Name;
        }

        public decimal CalculatePremium()
        {
            return (risk.YearlyPrice / 12) * insurancePeriod.PremiumMonths;
        }
    }
}
