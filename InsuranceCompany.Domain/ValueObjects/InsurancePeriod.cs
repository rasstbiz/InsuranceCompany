using System;

namespace Domain
{
    public class InsurancePeriod
    {
        public DateTime From { get; }

        public DateTime Till { get; }

        public short PremiumMonths { get; }

        public InsurancePeriod(DateTime validFrom, short validMonths)
        {
            if (validMonths < 1)
            {
                throw new InvalidInsurancePeriodException("At least one Valid Month should be provided");
            }

            From = validFrom.Date;
            Till = validFrom.Date.AddMonths(validMonths);
            PremiumMonths = validMonths;
        }

        public InsurancePeriod(DateTime validFrom, DateTime validTill)
        {
            if (validTill < DateTime.Now.Date)
            {
                throw new InvalidInsurancePeriodException("Valid Till date can not be in the past");
            }
            if (validFrom > validTill)
            {
                throw new InvalidInsurancePeriodException("Valid From date can not be greater that Valid Till date");
            }

            From = validFrom.Date;
            Till = validTill.Date;

            var months = (short)(((validTill.Year - validFrom.Year) * 12) + validTill.Month - validFrom.Month);
            PremiumMonths = (short)(months == 0 ? 1 : months);
        }
    }
}
