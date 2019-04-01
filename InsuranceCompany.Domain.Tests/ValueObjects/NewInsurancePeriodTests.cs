using AutoFixture;
using Domain;
using System;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class NewInsurancePeriodShould
    {
        private readonly IFixture fixture;

        public NewInsurancePeriodShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Fact]
        public void ThrowWhenValidFromDateIsInThePast_ValidMonthsConstructor()
        {
            var validFromInThePast = DateTime.Now.AddDays(-1);

            Assert.Throws<InvalidInsurancePeriodException>(() =>
                new NewInsurancePeriod(validFromInThePast, fixture.Create<short>()));
        }

        [Fact]
        public void ThrowWhenValidFromDateIsInThePast_ValidTillConstructor()
        {
            var validFromInThePast = DateTime.Now.AddDays(-1);

            Assert.Throws<InvalidInsurancePeriodException>(() =>
                new NewInsurancePeriod(validFromInThePast, DateTime.Now.AddDays(1)));
        }
    }
}
