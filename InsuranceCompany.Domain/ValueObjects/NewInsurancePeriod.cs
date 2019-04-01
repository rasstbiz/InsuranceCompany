using System;

namespace Domain
{
    public class NewInsurancePeriod : InsurancePeriod
    {
        public NewInsurancePeriod(DateTime validFrom, short validMonths) 
            : base(validFrom, validMonths)
        {
            if (validFrom < DateTime.Now.Date)
            {
                throw new InvalidInsurancePeriodException("Valid From date can not be in the past");
            }
        }

        public NewInsurancePeriod(DateTime validFrom, DateTime validTill) 
            : base(validFrom, validTill)
        {
            if (validFrom < DateTime.Now.Date)
            {
                throw new InvalidInsurancePeriodException("Valid From date can not be in the past");
            }
        }
    }
}
