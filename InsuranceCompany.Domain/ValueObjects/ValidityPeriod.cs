using System;

namespace Domain
{
    public class ValidityPeriod
    {
        public DateTime From { get; }

        public DateTime Till { get; }

        public short Months { get; }

        public ValidityPeriod(DateTime validFrom, short validMonths)
        {
            if (validMonths < 1)
            {
                throw new InvalidValidityPeriodException("At least one Valid Month should be provided");
            }

            From = validFrom.Date;
            Till = validFrom.AddMonths(validMonths);
            Months = validMonths;
        }

        public ValidityPeriod(DateTime validFrom, DateTime validTill)
        {
            if (validTill < DateTime.Now.Date)
            {
                throw new InvalidValidityPeriodException("Valid Till date can not be in the past");
            }

            From = validFrom.Date;
            Till = validTill.Date;
            Months = (short)(((validTill.Year - validFrom.Year) * 12) + validTill.Month - validFrom.Month);
        }
    }
}
