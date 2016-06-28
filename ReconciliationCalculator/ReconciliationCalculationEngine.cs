
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReconciliationCalculator
{
    

    public class ReconciliationCalculationEngine
    {
        public  void CalculateReconciliation(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup, PolicyConfiguration policyConfiguration)
        {
            CalculatePlannedClaimedAndVarianceAtDeliverableGroupLevel(reconciliationAllocationGroup);

            CapClaimedValueAtDeliverableGroupLevelIfApplicable(reconciliationAllocationGroup, policyConfiguration);

            CalculatePlannedClaimedAndVarianceAtAllocationLevel(reconciliationAllocationGroup);

            CalculatePlannedClaimedAndVarianceAtClaimGroupLevel(reconciliationAllocationGroup);

            CalculateReconciliationAtReconciliationAllocationGroupLevel(reconciliationAllocationGroup, policyConfiguration);

            CalculateReconciliationAtAllocationLevel(reconciliationAllocationGroup);

        }

        private void CalculatePlannedClaimedAndVarianceAtDeliverableGroupLevel(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup)
        {
            reconciliationAllocationGroup.ContractAllocations.ForEach(
                alloc =>
                    alloc.DeliverableGroups.ForEach(
                        delGroup =>
                        {
                            delGroup.PlannedValue =
                                delGroup.Deliverables.Sum(x => x.PlannedValue.GetValueOrDefault())
                            ;
                            delGroup.ClaimedValue = delGroup.Deliverables.Sum(x => x.ActualsToDateValue.GetValueOrDefault() + x.ForecastRemainingDeliveryValue.GetValueOrDefault() + x.ExceptionalAdjustmentsValue.GetValueOrDefault());
                            delGroup.CappedClaimValue = delGroup.ClaimedValue;
                            delGroup.VarianceValue = delGroup.ClaimedValue.GetValueOrDefault() - delGroup.PlannedValue.GetValueOrDefault();
                        }));

        }
        private void CalculatePlannedClaimedAndVarianceAtAllocationLevel(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup)
        {
            reconciliationAllocationGroup.ContractAllocations.ForEach(allocation =>
            {
                allocation.PlannedValue =
                    allocation.DeliverableGroups.Sum(delGroup => delGroup.PlannedValue.GetValueOrDefault());

                allocation.ClaimedValue =
                    allocation.DeliverableGroups.Sum(delGroup => delGroup.ClaimedValue.GetValueOrDefault());

                allocation.CappedClaimValue =
                    allocation.DeliverableGroups.Sum(delGroup => delGroup.CappedClaimValue.GetValueOrDefault());

                allocation.VarianceValue =
                    allocation.DeliverableGroups.Sum(delGroup => delGroup.VarianceValue.GetValueOrDefault());

                allocation.Providerperformance = GetProviderPerformance(allocation.VarianceValue);
                
            });
        }

        private ProviderPerformance GetProviderPerformance(decimal? varianceValue)
        {
            var retValue = ProviderPerformance.OnTrack;

            if (varianceValue.GetValueOrDefault() > 0)
                retValue= ProviderPerformance.OverDeliver;
            else if (varianceValue.GetValueOrDefault() < 0)
                retValue= ProviderPerformance.UnderDeliver;

            return retValue;
                
        }

        private void CalculateReconciliationAtAllocationLevel(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup)
        {
            if (reconciliationAllocationGroup.ProviderPerformance == ProviderPerformance.OnTrack)
                return;

             var allocations =
                    reconciliationAllocationGroup.ContractAllocations.Where(
                        x => x.Providerperformance == reconciliationAllocationGroup.ProviderPerformance);

            var contractAllocations = allocations as IList<ContractAllocation> ?? allocations.ToList();
            var totalVariance = contractAllocations.Sum(x => x.VarianceValue);
            
            foreach (var allocation in contractAllocations)
            {
                allocation.ReconciledValue = allocation.VarianceValue.GetValueOrDefault() /
                                              totalVariance *
                                             reconciliationAllocationGroup.ReconciledValue.GetValueOrDefault();

                allocation.ReconciledValue = Math.Round(allocation.ReconciledValue.GetValueOrDefault(), 2);
                allocation.ProposedReconciledValue = allocation.ReconciledValue;
            }
            
        }

        private void CalculateReconciliationAtReconciliationAllocationGroupLevel(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup, PolicyConfiguration policyConfiguration)
        {
            var reconciliationAllocationGroupSetting =
                       policyConfiguration.ClaimGroupSettings.FirstOrDefault(x => x.ReconciliationAllocationGroupName == reconciliationAllocationGroup.ReconciliationAllocationGroupName && x.ClaimTypeName==reconciliationAllocationGroup.ClaimTypeName);

            if (reconciliationAllocationGroupSetting == null) return;

            if ((!reconciliationAllocationGroupSetting.ReconcileIfOverDelivery ||
                 reconciliationAllocationGroup.ProviderPerformance != ProviderPerformance.OverDeliver) &&
                (!reconciliationAllocationGroupSetting.ReconcileIfUnderDelivery ||
                 reconciliationAllocationGroup.ProviderPerformance != ProviderPerformance.UnderDeliver)) return;

            var deminimis = reconciliationAllocationGroup.ProviderPerformance == ProviderPerformance.OverDeliver
                ? reconciliationAllocationGroupSetting.OverDeliveryDeminimis.GetValueOrDefault()
                : reconciliationAllocationGroupSetting.UnderDeliveryDeminimis.GetValueOrDefault();

            if (Math.Abs(reconciliationAllocationGroup.VarianceValue.GetValueOrDefault()) <= Math.Abs(deminimis)) return;
            reconciliationAllocationGroup.ReconciledValue = reconciliationAllocationGroup.VarianceValue;
        }

        private void CalculatePlannedClaimedAndVarianceAtClaimGroupLevel(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup)
        {
            reconciliationAllocationGroup.PlannedValue =
                reconciliationAllocationGroup.ContractAllocations.Sum(x => x.PlannedValue.GetValueOrDefault());

            reconciliationAllocationGroup.ClaimedValue =
                reconciliationAllocationGroup.ContractAllocations.Sum(x => x.ClaimedValue.GetValueOrDefault());

            reconciliationAllocationGroup.CappedClaimValue =
               reconciliationAllocationGroup.ContractAllocations.Sum(x => x.CappedClaimValue.GetValueOrDefault());

            reconciliationAllocationGroup.VarianceValue = 
                reconciliationAllocationGroup.ContractAllocations.Sum(x => x.VarianceValue.GetValueOrDefault());

            reconciliationAllocationGroup.ProviderPerformance = GetProviderPerformance(reconciliationAllocationGroup.VarianceValue);
        }

        private void CapClaimedValueAtDeliverableGroupLevelIfApplicable(FundingClaimReconciliationAllocationGroup reconciliationAllocationGroup, PolicyConfiguration policyConfiguration)
        {
            foreach (ContractAllocation allocation in reconciliationAllocationGroup.ContractAllocations)
                foreach (var deliverableGroup in allocation.DeliverableGroups)
                {
                    DeliverableGroupBehaviour deliverableGroupSetting = policyConfiguration.DeliverableGroupSettings.FirstOrDefault(x => x.DeliverableGroupName == deliverableGroup.DeliverableGroupName);
                    if (deliverableGroupSetting != null)
                    {
                        if (deliverableGroupSetting.IsClaimToProfileCappingNeeded && deliverableGroup.ClaimedValue > deliverableGroup.PlannedValue)
                        {
                            deliverableGroup.CappedClaimValue = deliverableGroup.PlannedValue.GetValueOrDefault();
                            deliverableGroup.VarianceValue = 0;
                        }
                    }
                }
        }
    }
}
