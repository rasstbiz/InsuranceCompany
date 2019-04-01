using System.Linq;

namespace Domain
{
    public class InsuranceCompanyService : IInsuranceCompanyService
    {
        private readonly IInsuranceCompany insuranceCompany;
        private readonly IAvailableRiskRepository availableRiskRepository;

        public InsuranceCompanyService(
            IInsuranceCompany insuranceCompany,
            IAvailableRiskRepository availableRiskRepository)
        {
            this.insuranceCompany = insuranceCompany;
            this.availableRiskRepository = availableRiskRepository;
        }

        public IInsuranceCompany Get()
        {
            insuranceCompany.AvailableRisks = availableRiskRepository.All()
                .Select(a => new Risk
                {
                    Name = a.Name,
                    YearlyPrice = a.YearlyPrice
                })
                .ToList();
            return insuranceCompany;
        }
    }
}
