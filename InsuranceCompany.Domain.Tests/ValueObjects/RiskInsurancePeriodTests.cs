using AutoFixture;
using Domain;
using System;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class RiskInsurancePeriodShould
    {
        private readonly IFixture fixture;

        public RiskInsurancePeriodShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Fact]
        public void ThrowIfInsurancePeriodIsNull()
        {
            InsurancePeriod nullInsurancePeriod = null;
            Risk risk = fixture.Create<Risk>();

            Assert.Throws<ArgumentNullException>(() =>
                new RiskInsurancePeriod(nullInsurancePeriod, risk));
        }

        [Fact]
        public void PropagateRiskName()
        {
            var validFrom = DateTime.Now;
            var validTill = DateTime.Now.AddDays(1);
            var insurancePeriod = new InsurancePeriod(validFrom, validTill);
            var risk = fixture.Create<Risk>();

            var riskInsurancePeriod = new RiskInsurancePeriod(insurancePeriod, risk);

            Assert.Equal(risk.Name, riskInsurancePeriod.RiskName);
        }

        [Fact]
        public void CalculatePremiumForEachValidMonth()
        {
            var validFrom = DateTime.Now;
            var validTill = DateTime.Now.AddMonths(2);
            var insurancePeriod = new InsurancePeriod(validFrom, validTill);
            var risk = new Risk
            {
                Name = fixture.Create<string>(),
                YearlyPrice = 120
            };

            var riskInsurancePeriod = new RiskInsurancePeriod(insurancePeriod, risk);

            var premium = riskInsurancePeriod.CalculatePremium();

            Assert.Equal(20, premium);
        }
    }
}
