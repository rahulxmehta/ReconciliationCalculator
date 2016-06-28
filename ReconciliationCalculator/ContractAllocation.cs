using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconciliationCalculator
{
    public class ContractAllocation
    {
        public List<DeliverableGroup> DeliverableGroups;

        public string AllocationReference { get; set; }

        public decimal? PlannedValue { get; set; }
        public decimal? ClaimedValue { get; set; }

        public decimal? CappedClaimValue { get; set; }

        public decimal? VarianceValue { get; set; }

        public ProviderPerformance Providerperformance { get; set; }

        public decimal? ReconciledValue { get; set; }

        public decimal? ProposedReconciledValue { get; set; }
    }

    public enum ProviderPerformance
    {
        OverDeliver,
        UnderDeliver,
        OnTrack
        
    }
}
