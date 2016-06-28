using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconciliationCalculator
{
    public class FundingClaimReconciliationAllocationGroup
    {
        public string ReconciliationAllocationGroupName { get; set; }

        public string ClaimTypeName { get; set; }

        public List<ContractAllocation> ContractAllocations { get; set; }

        public decimal? PlannedValue { get; set; }
        public decimal? ClaimedValue { get; set; }
        public decimal? CappedClaimValue { get; set; }
        public decimal? VarianceValue { get; set; }

        public decimal? ReconciledValue { get; set; }

        public ProviderPerformance ProviderPerformance { get; set; }
        
    }
}
