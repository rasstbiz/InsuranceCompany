using AutoFixture;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class RiskPackageShould
    {
        private readonly IFixture fixture;

        public RiskPackageShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Fact]
        public void ThrowIfInsuredRisksAreEmpty()
        {
            var validityPeriod = new ValidityPeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = new List<Risk>();

            Assert.Throws<MissingInitialInsuredRisksException>(() => 
                new RiskPackage(insuredRisks, validityPeriod));
        }

        [Fact]
        public void ThrowIfValidityPeriodIsNull()
        {
            ValidityPeriod validityPeriod = null;
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            Assert.Throws<ArgumentNullException>(() =>
                new RiskPackage(insuredRisks, validityPeriod));
        }

        [Fact]
        public void CalculatePremiumForInitialSetOfRisks()
        {
            var validityPeriod = new ValidityPeriod(DateTime.Now, 12);
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

            var riskPackage = new RiskPackage(insuredRisks, validityPeriod);

            var premium = riskPackage.CalculatePremium();

            Assert.Equal(76m, premium);
        }

        [Fact]
        public void CalculatePremiumForInitialAndAdditionalSetOfRisks()
        {
            var additionalValidityPeriod = new ValidityPeriod(DateTime.Now.AddMonths(6), 6);
            var additionalRisk = new Risk
            {
                Name = fixture.Create<string>(),
                YearlyPrice = 50
            };
            var additionalInsuredPeriod = new RiskValidityPeriod(additionalValidityPeriod, additionalRisk);
            var validityPeriod = new ValidityPeriod(DateTime.Now, 12);
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

            var riskPackage = new RiskPackage(insuredRisks, validityPeriod, additionalInsuredPeriod);

            var premium = riskPackage.CalculatePremium();

            Assert.Equal(101m, premium);
        }
    }
}
