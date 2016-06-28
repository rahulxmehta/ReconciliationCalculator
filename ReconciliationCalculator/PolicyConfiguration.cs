using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconciliationCalculator
{
    public class PolicyConfiguration
    {
        public List<ReconciliationAllocationGroupClaimTypeBehaviour> ClaimGroupSettings { get; set; }
        public List<DeliverableGroupBehaviour> DeliverableGroupSettings { get; set; }
    }

    public class ReconciliationAllocationGroupClaimTypeBehaviour
    {
        public string ReconciliationAllocationGroupName { get; set; }
        public string ClaimTypeName { get; set; }
        public decimal? OverDeliveryDeminimis { get; set; }
        public decimal? UnderDeliveryDeminimis { get; set; }
        public bool ReconcileIfOverDelivery { get; set; }
        public bool ReconcileIfUnderDelivery { get; set; }
        
    }

    public class DeliverableGroupBehaviour
    {
        public string DeliverableGroupName { get; set; }
        public bool IsClaimToProfileCappingNeeded { get; set; }

    }
}
