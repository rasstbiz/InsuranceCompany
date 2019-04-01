using Domain;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class AvailableRiskRepository : IAvailableRiskRepository
    {
        public IEnumerable<AvailableRisk> All()
        {
            using (var db = new LiteDatabase(@"AvailableRisks.db"))
            {
                var riskCollection = db.GetCollection<AvailableRisk>("availableRisks");
                var risks = riskCollection.FindAll().ToList();
                if (!risks.Any())
                {
                    risks = new List<AvailableRisk>
                    {
                        new AvailableRisk
                        {
                            Name = "Data breaches",
                            YearlyPrice = 300
                        },
                        new AvailableRisk
                        {
                            Name = "Property damage",
                            YearlyPrice = 350
                        },
                        new AvailableRisk
                        {
                            Name = "Human capital costs",
                            YearlyPrice = 450
                        },
                        new AvailableRisk
                        {
                            Name = "Professional service mistakes",
                            YearlyPrice = 238
                        },
                        new AvailableRisk
                        {
                            Name = "International manufacturing and export/transit issues",
                            YearlyPrice = 539
                        },
                        new AvailableRisk
                        {
                            Name = "Building projects",
                            YearlyPrice = 219
                        }
                    };
                    riskCollection.InsertBulk(risks);
                }

                return risks;
            }
        }
    }
}
