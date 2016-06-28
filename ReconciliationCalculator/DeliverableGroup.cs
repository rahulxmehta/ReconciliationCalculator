using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconciliationCalculator
{
    public class DeliverableGroup
    {
        public string DeliverableGroupName { get; set; }

        public List<Deliverable> Deliverables;
        
        public decimal? PlannedValue { get; set; }
        public decimal? ClaimedValue { get; set; }
        public decimal? VarianceValue { get; set; }
        public decimal? CappedClaimValue { get; set; }
        
    }
}
