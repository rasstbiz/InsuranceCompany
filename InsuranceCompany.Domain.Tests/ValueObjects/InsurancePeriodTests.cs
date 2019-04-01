using AutoFixture;
using Domain;
using System;
using Xunit;

namespace InsurancyCompany.Domain.Tests
{
    public class InsurancePeriodShould
    {
        private readonly IFixture fixture;

        public InsurancePeriodShould()
        {
            fixture = new Fixture()
                .Customize(new SupportMutableValueTypesCustomization());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ThrowIfValidMonthsValueIsLessThanOne(short invalidMonthsValue)
        {
            Assert.Throws<InvalidInsurancePeriodException>(() => 
                new InsurancePeriod(DateTime.Now, invalidMonthsValue));
        }

        [Fact]
        public void ThrowIfValidTillDateValueIsInThePast()
        {
            var validTillDateInThePast = DateTime.Now.AddDays(-1);
            var validFromDate = DateTime.Now.AddMonths(-11);
            Assert.Throws<InvalidInsurancePeriodException>(() =>
                new InsurancePeriod(validFromDate, validTillDateInThePast));
        }

        [Fact]
        public void ThrowIfValidTillDateIsLessThanValidFromDate()
        {
            var validTillDateInThePast = DateTime.Now.AddDays(-1);
            var validFromDate = DateTime.Now.AddDays(1);
            Assert.Throws<InvalidInsurancePeriodException>(() =>
                new InsurancePeriod(validFromDate, validTillDateInThePast));
        }

        [Fact]
        public void CalculateTillValue()
        {
            short validMonths = 3;
            var validFromDate = DateTime.Now;
            var validTill = DateTime.Now.AddMonths(validMonths);

            var insurancePeriod = new InsurancePeriod(validFromDate, validMonths);

            Assert.Equal(validTill.Date, insurancePeriod.Till);
        }

        [Fact]
        public void CalculatePremiumMonthsValue()
        {
            short validMonths = 3;
            var validFromDate = DateTime.Now;
            var validTill = DateTime.Now.AddMonths(validMonths);

            var insurancePeriod = new InsurancePeriod(validFromDate, validMonths);

            Assert.Equal(validMonths, insurancePeriod.PremiumMonths);
        }

        [Fact]
        public void SetPremiumMonthsToOneIfPeriodIsTooShort()
        {
            var currentDate = DateTime.Now;
            var validFromDate = new DateTime(currentDate.Year + 1, 01, 01);
            var validTill = validFromDate.AddDays(1);

            var shortInsurancePeriod = new InsurancePeriod(validFromDate, validTill);

            Assert.Equal(1, shortInsurancePeriod.PremiumMonths);
        }
    }
}
