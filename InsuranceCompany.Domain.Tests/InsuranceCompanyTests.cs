using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Domain;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class InsuranceCompanyShould
{
    private readonly IFixture fixture;

    public InsuranceCompanyShould()
    {
        fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization())
            .Customize(new SupportMutableValueTypesCustomization());
    }

    [Fact]
    public void GetPolicyFromRepository()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        var policies = MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        var policy = insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate);

        Assert.Equal(policies.Single(), policy);
    }

    [Fact]
    public void ThrowWhenMultipleEffectivePoliciesFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<MultipleEffectivePoliciesFoundException>(() => 
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ThrowWhenNoEffectivePolicyFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ValidateEffectiveDateForPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnIneffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ThrowWhenValidityPeriodForNewPolicyClashesWithExisting()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var validMonths = fixture.Create<short>();
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<ExistingEffectivePolicyException>(() =>
            insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks));
    }

    [Fact]
    public void ThrowWhenValidFromDateIsInThePast()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(-1);
        var validMonths = fixture.Create<short>();
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<InvalidValidityPeriodException>(() =>
            insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ThrowWhenValidMonthsValueIsInvalid(short validMonths)
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<InvalidValidityPeriodException>(() =>
            insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks));
    }

    [Fact]
    public void ThrowWhenSomeOfTheSelectedRisksAreNotAvailableForSelling()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var validMonths = fixture.Create<short>();
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<InitialSelectedRiskUnavailableException>(() =>
            insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks));
    }

    [Fact]
    public void SellPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var validMonths = fixture.Create<short>();
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();
        insuranceCompany.AvailableRisks = selectedRisks;

        var policy = insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks);

        Assert.Equal(nameOfInsuredObject, policy.NameOfInsuredObject);
        Assert.Equal(validFrom, policy.ValidFrom);
        Assert.Equal(validFrom.AddMonths(validMonths), policy.ValidTill);
        Assert.Equal(selectedRisks, policy.InsuredRisks);
        Assert.True(policy.Premium > 0m);
    }

    private IEnumerable<IPolicy> MakeRepositoryReturnEffectivePolicies(
        string nameOfInsuredObject,
        DateTime effectiveDate,
        short count)
    {
        var policies = fixture.Build<Policy>()
            .With(p => p.ValidFrom, effectiveDate.AddDays(-1))
            .With(p => p.ValidTill, effectiveDate.AddDays(1))
            .CreateMany(count);

        fixture.Freeze<IPolicyRepository>()
            .FindByNameOfInsuredObject(nameOfInsuredObject)
            .Returns(policies);

        return policies;
    }

    private IEnumerable<IPolicy> MakeRepositoryReturnIneffectivePolicies(
        string nameOfInsuredObject,
        DateTime effectiveDate,
        short count)
    {
        var policies = fixture.Build<Policy>()
            .With(p => p.ValidFrom, effectiveDate.AddDays(-5))
            .With(p => p.ValidTill, effectiveDate.AddDays(-1))
            .CreateMany(count);

        fixture.Freeze<IPolicyRepository>()
            .FindByNameOfInsuredObject(nameOfInsuredObject)
            .Returns(policies);

        return policies;
    }
}