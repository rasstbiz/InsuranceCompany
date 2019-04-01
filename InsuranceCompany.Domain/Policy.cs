using System;
using System.Collections.Generic;

namespace Domain
{
    public class Policy : IPolicy
    {
        public Policy()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string NameOfInsuredObject { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTill { get; set; }

        public decimal Premium { get; set; }

        public IList<Risk> InsuredRisks { get; set; }
    }
}
