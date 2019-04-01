using System;

namespace WebApi
{
    public class AddRiskRequest
    {
        public string NameOfInsuredObject { get; set; }

        public Risk Risk { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
