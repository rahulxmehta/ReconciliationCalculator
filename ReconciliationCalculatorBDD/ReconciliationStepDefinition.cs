using ReconciliationCalculator;
using System;
using TechTalk.SpecFlow;

namespace ReconciliationCalculatorBDD
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [Binding]
    public class ReconciliationStepDefinition
    {
        private FundingClaimReconciliationAllocationGroup _claimGroup;
        private PolicyConfiguration _policyConfiguration;

        [Given(@"A funding claim reconciliation allocation group exists with name '(.*)' And Claim Type as '(.*)' that contains Allocation as follows")]
        public void GivenAFundingClaimGroupExistsWithNameAndClaimTypeAsThatContainsAllocationAsFollows(string ReconciliationAllocationGroupName, string claimTypeName, Table tblAllocations)
        {
            _claimGroup = new FundingClaimReconciliationAllocationGroup();
            _claimGroup.ReconciliationAllocationGroupName = ReconciliationAllocationGroupName;
            _claimGroup.ClaimTypeName = claimTypeName;
            _claimGroup.ContractAllocations = new List<ContractAllocation>();
            foreach (TableRow row in tblAllocations.Rows)
            {
                _claimGroup.ContractAllocations.Add(new ContractAllocation()
                {
                    AllocationReference = row["Allocation Reference"].ToString()
                });
            }
        }


        [Given(@"the Allocation Reference '(.*)' contains Deliverable Group '(.*)' that contains following deliverables")]
        public void GivenTheAllocationReferenceContainsDeliverableGroupThatContainsFollowingDeliverables(string allocationReference, string deliverableGroupName, Table tblDeliverables)
        {
            var contractAllocation =
                _claimGroup.ContractAllocations.FirstOrDefault(x => x.AllocationReference == allocationReference);

            if (contractAllocation == null)
            {
                Assert.Fail($"Contract Allocation with Allocation Reference {allocationReference} is not found");
            }

            var deliverables = tblDeliverables.Rows.Select(row => new Deliverable()
            {
                DeliverableCode = Convert.ToInt32(row["Deliverable Code"]),
                PlannedValue = Convert.ToDecimal(row["Deliverable Planned Value"]),
                ActualsToDateValue = Convert.ToDecimal(row["Deliverable Actuals to Date Value"]),
                ForecastRemainingDeliveryValue = Convert.ToDecimal(row["Deliverable Forecast Remaining Delivery Value"]),
                ExceptionalAdjustmentsValue = Convert.ToDecimal(row["Deliverable Exceptional Adjustments Value"])
            }).ToList();

            if (contractAllocation.DeliverableGroups == null)
            {
                contractAllocation.DeliverableGroups = new List<DeliverableGroup>();
            }

            DeliverableGroup deliverableGroup = new DeliverableGroup();
            deliverableGroup.DeliverableGroupName = deliverableGroupName;
            deliverableGroup.Deliverables = deliverables;
            contractAllocation.DeliverableGroups.Add(deliverableGroup);

        }
        
        
        
        [Given(@"Policy Configuration Exists for Reconciliation Allocation Group '(.*)' And Claim Type Name as '(.*)' with behaviour as :")]
        public void GivenPolicyConfigurationExistsForClaimGroupWithBehaviourAs(string ReconciliationAllocationGroupName, string claimTypeName, Table tblClaimGroupSettings)
        {
            TableRow row = tblClaimGroupSettings.Rows[0];

            _policyConfiguration = new PolicyConfiguration()
            {
                ClaimGroupSettings = new List<ReconciliationAllocationGroupClaimTypeBehaviour>()
                {
                    new ReconciliationAllocationGroupClaimTypeBehaviour()
                    {
                        ReconciliationAllocationGroupName = ReconciliationAllocationGroupName,
                        ClaimTypeName = claimTypeName,
                        OverDeliveryDeminimis =Convert.ToDecimal(row["OverDeliveryDeminimis"]),
                        UnderDeliveryDeminimis =Convert.ToDecimal(row["UnderDeliveryDeminimis"]),
                        ReconcileIfOverDelivery = GetBooleanValue(row["ReconcileOverDelivery"]),
                        ReconcileIfUnderDelivery = GetBooleanValue(row["ReconcileUnderDelivery"])
                    }
                }
            };
            
        }

        private bool GetBooleanValue(string value)
        {
            if (value == "True")
                return true;
            else
                return false;
            
        }

        [Given(@"Deliverable Group '(.*)' exists as follows")]
        public void GivenDeliverableGroupExistsAsFollows(string deliverableGroupName, Table tblDeliverableGroupSettings)
        {
            TableRow row =
                tblDeliverableGroupSettings.Rows[0];

            if  (_policyConfiguration == null)
                _policyConfiguration = new PolicyConfiguration();
            

            if (_policyConfiguration.DeliverableGroupSettings == null)
                _policyConfiguration.DeliverableGroupSettings = new List<DeliverableGroupBehaviour>();

            DeliverableGroupBehaviour groupBehaviour = new DeliverableGroupBehaviour
            {
                DeliverableGroupName = deliverableGroupName,
                IsClaimToProfileCappingNeeded = GetBooleanValue(row["IsClaimToProfileCappingNeeded"])
            };

            _policyConfiguration.DeliverableGroupSettings.Add(groupBehaviour);

        }
        
        [When(@"reconciliation is calculated")]
        public void WhenReconciliationIsCalculated()
        {
            var reconEngine = new ReconciliationCalculationEngine();
            reconEngine.CalculateReconciliation(_claimGroup,_policyConfiguration);
        }
        
        [Then(@"the result should be at level of funding claim group")]
        public void ThenTheResultShouldBeAtLevelOfFundingClaimGroup(Table table)
        {
            TableRow row = table.Rows[0];
            
            decimal expectedPlannedValue = Convert.ToDecimal(row["Planned Value"]);
            decimal expectedClaimedValue = Convert.ToDecimal(row["Claimed Value"]);
            decimal expectedCappedClaimedValue = Convert.ToDecimal(row["Capped Claimed Value"]);
            decimal expectedReconciliationValue = Convert.ToDecimal(row["Reconciliation Value"]);
            decimal expectedProposedReconciliationValue = Convert.ToDecimal(row["Proposed Reconciliation Value"]);

            Assert.AreEqual(expectedPlannedValue, _claimGroup.PlannedValue, $"Calculated Planned amount - {_claimGroup.PlannedValue}  doesn't matches with expected value - {expectedPlannedValue}");
            Assert.AreEqual(expectedClaimedValue, _claimGroup.ClaimedValue, $"Calculated Claim amount - {_claimGroup.ClaimedValue}  doesn't matches with expected value - {expectedClaimedValue}");
            Assert.AreEqual(expectedCappedClaimedValue, _claimGroup.CappedClaimValue, $"Calculated Capped Claim amount - {_claimGroup.CappedClaimValue}  doesn't matches with expected value - {expectedCappedClaimedValue}");
            Assert.AreEqual(expectedReconciliationValue, _claimGroup.ReconciledValue, $"Calculated Reconciliation amount - {_claimGroup.ReconciledValue}  doesn't matches with expected value - {expectedReconciliationValue}");
            Assert.AreEqual(expectedProposedReconciliationValue, _claimGroup.ReconciledValue, $"Calculated Proposed Reconciliation amount - {_claimGroup.ReconciledValue}  doesn't matches with expected value - {expectedProposedReconciliationValue}");
        }

        [Then(@"the result at the Allocation level should be")]
        public void ThenTheResultAtTheAllocationLevelShouldBe(Table table)
        {
            decimal? calculatedPlannedValue =  null;
            decimal? calculatedClaimedValue = null;
            decimal? calculatedCappedClaimedValue = null;
            decimal? calculatedReconciliationValue = null;
            decimal? calculatedProposedReconciliationValue = null;


            foreach (TableRow row in table.Rows)
            {
                decimal expectedPlannedValue = Convert.ToDecimal(row["Planned Value"]);
                decimal expectedClaimedValue = Convert.ToDecimal(row["Claimed Value"]);
                decimal expectedCappedClaimedValue = Convert.ToDecimal(row["Capped Claimed Value"]);
                decimal expectedReconciliationValue = Convert.ToDecimal(row["Reconciliation Value"]);
                decimal expectedProposedReconciliationValue = Convert.ToDecimal(row["Proposed Reconciliation Value"]);

                var firstOrDefault = _claimGroup.ContractAllocations.FirstOrDefault(x => x.AllocationReference == row["Allocation Reference"]);
                if (firstOrDefault == null)
                    Assert.Fail($"Contract Allocation with Allocation Reference {row["Allocation Reference"]} not found in the Claim");
                else
                { 
                    calculatedPlannedValue = firstOrDefault.PlannedValue;
                    calculatedClaimedValue = firstOrDefault.ClaimedValue;
                    calculatedCappedClaimedValue = firstOrDefault.CappedClaimValue;
                    calculatedReconciliationValue = firstOrDefault.ReconciledValue;
                    calculatedProposedReconciliationValue = firstOrDefault.ProposedReconciledValue;
                }

                Assert.AreEqual(expectedPlannedValue, calculatedPlannedValue, $"Calculated Planned amount - {calculatedPlannedValue}  doesn't matches with expected value - {expectedPlannedValue} for Allocation Reference - {row["Allocation Reference"]}");
                Assert.AreEqual(expectedClaimedValue, calculatedClaimedValue, $"Calculated Claimed amount - {calculatedClaimedValue}  doesn't matches with expected value - {expectedClaimedValue} for Allocation Reference - {row["Allocation Reference"]}");
                Assert.AreEqual(expectedCappedClaimedValue, calculatedCappedClaimedValue, $"Calculated Capped Claimed amount - {calculatedCappedClaimedValue}  doesn't matches with expected value - {expectedCappedClaimedValue} for Allocation Reference - {row["Allocation Reference"]}");
                Assert.AreEqual(expectedReconciliationValue, calculatedReconciliationValue, $"Calculated Reconciliation amount - {calculatedReconciliationValue}  doesn't matches with expected value - {expectedReconciliationValue} for Allocation Reference - {row["Allocation Reference"]}");
                Assert.AreEqual(expectedProposedReconciliationValue, calculatedProposedReconciliationValue, $"Calculated Proposed Reconciliation amount - {calculatedProposedReconciliationValue}  doesn't matches with expected value - {expectedProposedReconciliationValue} for Allocation Reference - {row["Allocation Reference"]}");
            }
            
        }
    }
}
