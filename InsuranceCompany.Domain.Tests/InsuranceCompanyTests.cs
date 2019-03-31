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

        var policies = MakeRepositoryReturnMultipleEffectivePolicies(nameOfInsuredObject, effectiveDate, 1);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        var policy = insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate);

        Assert.Equal(policies.Single(), policy);
    }

    [Fact]
    public void ThrowWhenMultipleEffectivePoliciesFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnMultipleEffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<MultipleEffectivePoliciesFoundException>(() => 
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ThrowWhenNoEffectivePolicyFoundForRequestedArgs()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnMultipleEffectivePolicies(nameOfInsuredObject, effectiveDate, 0);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    [Fact]
    public void ValidateEffectiveDateForPolicy()
    {
        var nameOfInsuredObject = fixture.Create<string>();
        var effectiveDate = fixture.Create<DateTime>();

        MakeRepositoryReturnMultipleIneffectivePolicies(nameOfInsuredObject, effectiveDate, 2);

        var insuranceCompany = fixture.Create<InsuranceCompany>();

        Assert.Throws<EffectivePolicyNotFoundException>(() =>
            insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
    }

    private IEnumerable<IPolicy> MakeRepositoryReturnMultipleEffectivePolicies(
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

    private IEnumerable<IPolicy> MakeRepositoryReturnMultipleIneffectivePolicies(
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