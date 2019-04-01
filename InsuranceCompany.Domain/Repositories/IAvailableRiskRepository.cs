using System.Collections.Generic;

namespace Domain
{
    public interface IAvailableRiskRepository
    {
        IEnumerable<AvailableRisk> All();
    }
}
