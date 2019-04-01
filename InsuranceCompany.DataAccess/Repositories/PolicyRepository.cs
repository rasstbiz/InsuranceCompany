using Domain;
using LiteDB;
using System.Collections.Generic;

namespace DataAccess
{
    public class PolicyRepository : IPolicyRepository
    {
        public IEnumerable<Policy> FindByNameOfInsuredObject(string nameOfInsuredObject)
        {
            using (var db = new LiteDatabase(@"Policies.db"))
            {
                var policies = db.GetCollection<Policy>("policies");
                return policies.Find(x => x.NameOfInsuredObject.Equals(nameOfInsuredObject));
            }
        }

        public void Add(Policy policy)
        {
            using (var db = new LiteDatabase(@"Policies.db"))
            {
                var policies = db.GetCollection<Policy>("policies");
                policies.Insert(policy);
                policies.EnsureIndex(p => p.NameOfInsuredObject);
            }
        }

        public void Update(Policy policy)
        {
            using (var db = new LiteDatabase(@"Policies.db"))
            {
                var policies = db.GetCollection<Policy>("policies");
                policies.Update(policy);
            }
        }
    }
}
