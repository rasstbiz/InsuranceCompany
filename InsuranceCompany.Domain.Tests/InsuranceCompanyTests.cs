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
        var effectiveDate = DateTime.Now.AddDays(1);

        var policies = MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        var policy = insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate);

        Assert.Equal(policies.Single(), policy);
    }

    [Fact]
    public void ThrowWhenMultipleEffectivePoliciesFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = DateTime.Now.AddDays(1);

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<MultipleEffectivePoliciesFoundException>(() => 
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ThrowWhenNoEffectivePolicyFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = DateTime.Now.AddDays(1);

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ValidateEffectiveDateForPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = DateTime.Now.AddDays(1);

        MakeRepositoryReturnIneffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ThrowWhenValidFromDateForNewPolicyClashesWithExisting()
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
    public void ThrowWhenValidTillDateForNewPolicyClashesWithExisting()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var validMonths = fixture.Create<short>();
        var validTill = validFrom.AddMonths(validMonths);
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validTill, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<ExistingEffectivePolicyException>(() =>
            insuranceCompany.SellPolicy(nameOfInsuredObject, validFrom, validMonths, selectedRisks));
    }

    [Fact]
    public void ThrowWhenWholeInsurancePeriodForNewPolicyIsOverlappedByExisting()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var validMonths = fixture.Create<short>();
        var validTill = validFrom.AddMonths(validMonths);
        var selectedRisks = fixture.CreateMany<Risk>().ToList();

        var policies = fixture.Build<Policy>()
            .With(p => p.ValidFrom, validFrom.AddDays(-1))
            .With(p => p.ValidTill, validTill.AddDays(1))
            .CreateMany(1);

        fixture.Freeze<IPolicyRepository>()
            .FindByNameOfInsuredObject(nameOfInsuredObject)
            .Returns(policies);

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

        Assert.Throws<InvalidInsurancePeriodException>(() =>
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

        Assert.Throws<InvalidInsurancePeriodException>(() =>
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

    [Fact]
    public void ThrowIfAddedRiskIsNotAvailable()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var effectiveDate = validFrom;
        var risk = fixture.Create<Risk>();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, validFrom, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<AddedRiskUnavailableException>(() =>
            insuranceCompany.AddRisk(nameOfInsuredObject, risk, validFrom, effectiveDate));
    }

    [Fact]
    public void ThrowWhenNoEffectivePolicyFoundForAddedRisk()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var effectiveDate = validFrom;
        var risk = fixture.Create<Risk>();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();
        insuranceCompany.AvailableRisks = new List<Risk> { risk };

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.AddRisk(nameOfInsuredObject, risk, validFrom, effectiveDate));
    }

    [Fact]
    public void AddRiskToExistingPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validFrom = DateTime.Now.Date.AddDays(1);
        var effectiveDate = validFrom;
        var risk = fixture.Create<Risk>();

        var policy = MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 1).First();

        var insuranceCompany = fixture.Create<InsuranceCompany>();
        insuranceCompany.AvailableRisks = new List<Risk> { risk };

        insuranceCompany.AddRisk(nameOfInsuredObject, risk, validFrom, effectiveDate);

        Assert.True(policy.InsuredRisks.Contains(risk));
    }

    [Fact]
    public void ThrowWhenNoEffectivePolicyFoundForRemovedRisk()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validTill = DateTime.Now.Date.AddDays(10);
        var effectiveDate = DateTime.Now;
        var risk = fixture.Create<Risk>();

        MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.RemoveRisk(nameOfInsuredObject, risk, validTill, effectiveDate));
    }

    [Fact]
    public void RemoveRiskFromExistingPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var validTill = DateTime.Now.Date.AddDays(50);
        var effectiveDate = DateTime.Now;
        var risk = fixture.Create<Risk>();

        var policy = MakeRepositoryReturnEffectivePolicies(nameOfInsuredObject, effectiveDate, 1).First();
        policy.InsuredRisks.Add(risk);
        policy.ValidTill = validTill.AddMonths(2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        insuranceCompany.RemoveRisk(nameOfInsuredObject, risk, validTill, effectiveDate);

        Assert.True(!policy.InsuredRisks.Contains(risk));
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