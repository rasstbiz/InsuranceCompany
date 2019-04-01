using AutoFixture;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class PolicyAggregateShould
    {
        private readonly IFixture fixture;

        public PolicyAggregateShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowIfInsuredObjectNameIsNullOrEmpty(string nameOfInsuredObject)
        {
            var policy = new Policy();
            var insurancePeriod = new InsurancePeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<MissingNameOfInsuredObjectException>(() => 
                policyAggregate.Create(nameOfInsuredObject, insurancePeriod, insuredRisks));
        }
        
        [Fact]
        public void ThrowIfRisksAreNotSelected()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var insurancePeriod = new InsurancePeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = new List<Risk>();

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<MissingInitialInsuredRisksException>(() =>
                policyAggregate.Create(nameOfInsuredObject, insurancePeriod, insuredRisks));
        }

        [Fact]
        public void PropagateArgumentsToEntityOnCreation()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var insurancePeriod = new InsurancePeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.Create(nameOfInsuredObject, insurancePeriod, insuredRisks);

            Assert.Equal(nameOfInsuredObject, policy.NameOfInsuredObject);
            Assert.Equal(insurancePeriod.From, policy.ValidFrom);
            Assert.Equal(insurancePeriod.Till, policy.ValidTill);
            Assert.Equal(insuredRisks, policy.InsuredRisks);
        }

        [Fact]
        public void CalculatePremiumOnCreation()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var insurancePeriod = new InsurancePeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.Create(nameOfInsuredObject, insurancePeriod, insuredRisks);

            Assert.True(policy.Premium > 0m);
        }

        [Fact]
        public void RecalculatePremiumForNewlyAddedRisk()
        {
            var validFrom = DateTime.Now;
            var risk = fixture.Create<Risk>();
            var policy = fixture
                .Build<Policy>()
                .With(p => p.ValidTill, validFrom.AddMonths(3))
                .Create();
            var originalPremium = policy.Premium;

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.AddRisk(risk, validFrom);

            Assert.NotEqual(originalPremium, policy.Premium);
        }

        [Fact]
        public void ThrowIfAddedRiskAlreadyInsured()
        {
            var validFrom = DateTime.Now;
            var risk = fixture.Create<Risk>();
            var policy = fixture
                .Build<Policy>()
                .With(p => p.ValidTill, validFrom.AddMonths(3))
                .With(p => p.InsuredRisks, new List<Risk> { risk })
                .Create();
            var originalPremium = policy.Premium;

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<ExistingRiskException>(() => 
                policyAggregate.AddRisk(risk, validFrom));
        }

        [Fact]
        public void ThrowIfRemovedRiskIsNotInsured()
        {
            var validTill = DateTime.Now.AddDays(100);
            var risk = fixture.Create<Risk>();
            var policy = fixture.Create<Policy>();

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<InvalidRiskException>(() =>
                policyAggregate.RemoveRisk(risk, validTill));
        }

        [Fact]
        public void RecalculatePremiumForRemovedRisk()
        {
            var validTill = DateTime.Now;
            var risk = fixture.Create<Risk>();
            var policy = fixture
                .Build<Policy>()
                .With(p => p.ValidTill, validTill.AddMonths(3))
                .With(p => p.InsuredRisks, new List<Risk> { risk })
                .Create();
            var originalPremium = policy.Premium;

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.RemoveRisk(risk, validTill);

            Assert.NotEqual(originalPremium, policy.Premium);
        }
    }
}
