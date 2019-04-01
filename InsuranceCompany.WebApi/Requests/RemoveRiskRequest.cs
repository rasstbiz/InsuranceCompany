using System;

namespace WebApi
{
    public class RemoveRiskRequest
    {
        public string NameOfInsuredObject { get; set; }

        public Risk Risk { get; set; }

        public DateTime ValidTill { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
