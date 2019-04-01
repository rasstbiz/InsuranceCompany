using System;

namespace Domain
{
    public class AvailableRisk
    {
        public AvailableRisk()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal YearlyPrice { get; set; }
    }
}
