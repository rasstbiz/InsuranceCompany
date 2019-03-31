using System;

namespace Domain
{
    public class ValidityPeriod
    {
        public DateTime From { get; }

        public DateTime Till { get; }

        public ValidityPeriod(DateTime validFrom, short validMonths)
        {
            if (validFrom < DateTime.Now.Date)
            {
                throw new InvalidValidityPeriodException("Valid From date can not be in the past");
            }
            if (validMonths < 1)
            {
                throw new InvalidValidityPeriodException("At least one Valid Month should be provided");
            }

            From = validFrom.Date;
            Till = validFrom.AddMonths(validMonths);
        }
    }
}
