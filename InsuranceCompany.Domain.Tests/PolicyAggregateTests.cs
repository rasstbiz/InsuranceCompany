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
            var validityPeriod = new ValidityPeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<MissingNameOfInsuredObjectException>(() => 
                policyAggregate.Create(nameOfInsuredObject, validityPeriod, insuredRisks));
        }
        
        [Fact]
        public void ThrowIfRisksAreNotSelected()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var validityPeriod = new ValidityPeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = new List<Risk>();

            var policyAggregate = new PolicyAggregate(policy);

            Assert.Throws<MissingInitialInsuredRisksException>(() =>
                policyAggregate.Create(nameOfInsuredObject, validityPeriod, insuredRisks));
        }

        [Fact]
        public void PropagateArgumentsToEntityOnCreation()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var validityPeriod = new ValidityPeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.Create(nameOfInsuredObject, validityPeriod, insuredRisks);

            Assert.Equal(nameOfInsuredObject, policy.NameOfInsuredObject);
            Assert.Equal(validityPeriod.From, policy.ValidFrom);
            Assert.Equal(validityPeriod.Till, policy.ValidTill);
            Assert.Equal(insuredRisks, policy.InsuredRisks);
        }

        [Fact]
        public void CalculatePremiumOnCreation()
        {
            var policy = new Policy();
            var nameOfInsuredObject = fixture.Create<string>();
            var validityPeriod = new ValidityPeriod(DateTime.Now, fixture.Create<short>());
            var insuredRisks = fixture.CreateMany<Risk>().ToList();

            var policyAggregate = new PolicyAggregate(policy);

            policyAggregate.Create(nameOfInsuredObject, validityPeriod, insuredRisks);

            Assert.True(policy.Premium > 0m);
        }
    }
}
