using System.Collections.Generic;

namespace Domain
{
    public interface IPolicyRepository
    {
        IEnumerable<IPolicy> FindByNameOfInsuredObject(string nameOfInsuredObject);
    }
}
