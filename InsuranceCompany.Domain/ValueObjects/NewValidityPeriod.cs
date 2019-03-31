using System;

namespace Domain
{
    public class NewValidityPeriod : ValidityPeriod
    {
        public NewValidityPeriod(DateTime validFrom, short validMonths) 
            : base(validFrom, validMonths)
        {
            if (validFrom < DateTime.Now.Date)
            {
                throw new InvalidValidityPeriodException("Valid From date can not be in the past");
            }
        }

        public NewValidityPeriod(DateTime validFrom, DateTime validTill) 
            : base(validFrom, validTill)
        {
            if (validFrom < DateTime.Now.Date)
            {
                throw new InvalidValidityPeriodException("Valid From date can not be in the past");
            }
        }
    }
}
