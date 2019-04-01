using System;

namespace WebApi
{
    public class SellRequest
    {
        public string NameOfInsuredObject { get; set; }

        public DateTime ValidFrom { get; set; }

        public short ValidMonths { get; set; }

        public Risk[] SelectedRisks { get; set; }
    }
}
