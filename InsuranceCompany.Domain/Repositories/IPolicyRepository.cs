using System.Collections.Generic;

namespace Domain
{
    public interface IPolicyRepository
    {
        IEnumerable<Policy> FindByNameOfInsuredObject(string nameOfInsuredObject);

        void Add(Policy policy);

        void Update(Policy policy);
    }
}
