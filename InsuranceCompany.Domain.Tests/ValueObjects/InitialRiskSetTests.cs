using AutoFixture;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class InitialRiskSetShould
    {
        private readonly IFixture fixture;

        public InitialRiskSetShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Fact]
        public void ThrowIfInsuredRisksAreEmpty()
        {
            var insurancePeriod = new InsurancePeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = new List<Risk>();

            Assert.Throws<MissingInitialInsuredRisksException>(() => 
                new InitialRiskSet(insuredRisks, insurancePeriod));
        }

        [Fact]
        public void ThrowIfinsurancePeriodIsNull()
        {
            InsurancePeriod insurancePeriod = null;
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            Assert.Throws<ArgumentNullException>(() =>
                new InitialRiskSet(insuredRisks, insurancePeriod));
        }

        [Fact]
        public void CalculatePremiumForInitialSetOfRisks()
        {
            var insurancePeriod = new InsurancePeriod(DateTime.Now, 12);
            var insuredRisks = new List<Risk>
            {
                new Risk
                {
                    Name = fixture.Create<string>(),
                    YearlyPrice = 40m
                },
                new Risk
                {
                    Name = fixture.Create<string>(),
                    YearlyPrice = 36m
                }
            };

            var initialRiskSet = new InitialRiskSet(insuredRisks, insurancePeriod);

            var premium = initialRiskSet.CalculatePremium();

            Assert.Equal(76m, premium);
        }

        [Fact]
        public void CalculatePremiumForInitialAndAdditionalSetOfRisks()
        {
            var additionalinsurancePeriod = new InsurancePeriod(DateTime.Now.AddMonths(6), 6);
            var additionalRisk = new Risk
            {
                Name = fixture.Create<string>(),
                YearlyPrice = 50
            };
            var riskInsuredPeriod = new RiskInsurancePeriod(additionalinsurancePeriod, additionalRisk);
            var insurancePeriod = new InsurancePeriod(DateTime.Now, 12);
            var insuredRisks = new List<Risk>
            {
                new Risk
                {
                    Name = fixture.Create<string>(),
                    YearlyPrice = 40m
                },
                new Risk
                {
                    Name = fixture.Create<string>(),
                    YearlyPrice = 36m
                }
            };

            var initialRiskSet = new InitialRiskSet(insuredRisks, insurancePeriod, riskInsuredPeriod);

            var premium = initialRiskSet.CalculatePremium();

            Assert.Equal(101m, premium);
        }
    }
}
