using System.Collections.Generic;

namespace Domain
{
    public class PolicyAggregate
    {
        private readonly IPolicy policy;
        private RiskPackage riskPackage;

        public PolicyAggregate(IPolicy policy)
        {
            this.policy = policy;
            this.riskPackage = null;
        }

        public void Create(string nameOfInsuredObject, ValidityPeriod validityPeriod, IList<Risk> insuredRisks)
        {
            if (string.IsNullOrEmpty(nameOfInsuredObject))
            {
                throw new MissingNameOfInsuredObjectException("Missing Name of Insured Object");
            }

            riskPackage = new RiskPackage(insuredRisks, validityPeriod);

            policy.NameOfInsuredObject = nameOfInsuredObject;
            policy.ValidFrom = validityPeriod.From;
            policy.ValidTill = validityPeriod.Till;
            policy.InsuredRisks = insuredRisks;
            policy.Premium = riskPackage.Sum();
        }
    }
}
