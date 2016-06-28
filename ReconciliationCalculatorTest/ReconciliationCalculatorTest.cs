using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReconciliationCalculatorTest
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ReconciliationCalculator;

    [TestClass]
    public class ReconciliationCalculatorTest
    {
        #region Community Learning Tests (Single Del/Single Alloc/Single Del Group)
        [TestMethod]
        [TestCategory("Community Learning")]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithSingleDelGroup_EqualPlannedAndClaimed()
        {

            var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 1000.0m,null,null);
            var policyConfiguration = CreatePolicyConfiguration();

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, policyConfiguration);
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(),"Reconcilialtion Value is incorrect");
        }

        [TestCategory("Community Learning")]
        [TestMethod]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithSingleDelGroup_VarianceLTUnderDeliveryDeminimis()
        {

            var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 900.0m,null,null);
            var policyConfiguration = CreatePolicyConfiguration();

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, policyConfiguration);
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("Community Learning")]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithSingleDelGroup_VarianceGTUnderDeliveryDeminimis()
        {

            var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 899.0m,null,null);
            var policyConfiguration = CreatePolicyConfiguration();

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, policyConfiguration);
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("Community Learning")]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithSingleDelGroup_ClawbackAll()
        {

            var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 0.0m, 0.0m, 0.0m);
            var policyConfiguration = CreatePolicyConfiguration();

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, policyConfiguration);
            Assert.AreEqual(-1000.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("Community Learning")]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithSingleDelGroup_OverDelivered()
        {

            var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 8000.0m,2000.0m,1000.0m);
            var policyConfiguration = CreatePolicyConfiguration();

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, policyConfiguration);
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        private FundingClaimReconciliationAllocationGroup CreateCommunityLearningFundingClaim(decimal plannedValue, decimal? actualsToDate, decimal? forecastRemainingDelivery, decimal? exceptionalAdjustments)
        {
            var deliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    PlannedValue = plannedValue,
                    ActualsToDateValue = actualsToDate,
                    ForecastRemainingDeliveryValue = forecastRemainingDelivery,
                    ExceptionalAdjustmentsValue = exceptionalAdjustments
                }
            };


            var deliverableGroups = new List<DeliverableGroup>
            {
                new DeliverableGroup()
                {
                    DeliverableGroupName = "CommunityLearning",
                    Deliverables = deliverables
                },
            };


            var contractAllocations = new List<ContractAllocation>
            {
                new ContractAllocation() {DeliverableGroups = deliverableGroups}
            };



            var claimGroup = new FundingClaimReconciliationAllocationGroup()
            {
                ReconciliationAllocationGroupName = "CommunityLearning1516",
                ClaimTypeName = "MID YEAR",
                ContractAllocations = contractAllocations
            };

            return claimGroup;
           
        }

        #endregion Community Learning Tests (Single Del/Single Alloc/Single Del Group)

        #region Adult Skills Tests (Multiple Del, Single Allocation, Multiple Del Group

        private Deliverable CreateDeliverable(int deliverableCode, decimal? plannedValue=null,decimal?actualsToDateValue = null, decimal? forecastRemainingDeliveryValue = null,decimal? exceptionalAdjustmentsValue = null)
        {
            return
            new Deliverable()
            {
               DeliverableCode = deliverableCode,
               PlannedValue = plannedValue,
               ActualsToDateValue = actualsToDateValue,
               ForecastRemainingDeliveryValue = forecastRemainingDeliveryValue,
               ExceptionalAdjustmentsValue = exceptionalAdjustmentsValue
            };

        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:250.0M),
                CreateDeliverable(4),
                CreateDeliverable(5),
                CreateDeliverable(6),
                CreateDeliverable(7),
                CreateDeliverable(8),
                CreateDeliverable(9)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:500.0M),
                CreateDeliverable(11,actualsToDateValue:200.0M,forecastRemainingDeliveryValue: 50.0m,exceptionalAdjustmentsValue:40.0m),
                CreateDeliverable(12,actualsToDateValue:200.0M,forecastRemainingDeliveryValue: 70.0m),
                CreateDeliverable(13),
                CreateDeliverable(14),
                CreateDeliverable(15),
                CreateDeliverable(16),
                CreateDeliverable(17),
                CreateDeliverable(18),
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-600.0M, claimGroup.ReconciledValue.GetValueOrDefault(),  "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("Adult Skills")]
        public void Scenario_1_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4),
                CreateDeliverable(5),
                CreateDeliverable(6),
                CreateDeliverable(7),
                CreateDeliverable(8),
                CreateDeliverable(9)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(13),
                CreateDeliverable(14),
                CreateDeliverable(15),
                CreateDeliverable(16),
                CreateDeliverable(17),
                CreateDeliverable(18),
            };
            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_2_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:250.0M,forecastRemainingDeliveryValue:150.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:800.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
            };
            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_3_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:300.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:49.0m),
           };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:800.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_4_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroup()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:250.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_5_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:250.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:49.0m),
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_6_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:399.0M)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue: 1000.0m,exceptionalAdjustmentsValue:500.0m),
                CreateDeliverable(12,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue: 1000.0m,exceptionalAdjustmentsValue:500.0m)

            };

            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Formatting = Formatting.Indented
            //};

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);
            //string json = JsonConvert.SerializeObject(claimGroup, settings);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            
            //json = JsonConvert.SerializeObject(claimGroup, settings);

            Assert.AreEqual(-101.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_7_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 50.0m,exceptionalAdjustmentsValue:50.0m)
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_8_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:750.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:49.0m)
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_9_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:51.0m)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:850.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(12,actualsToDateValue:750.0M,forecastRemainingDeliveryValue: 100.0m,exceptionalAdjustmentsValue:49.0m)
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestCategory("Adult Skills")]
        [TestMethod]
        public void Scenario_10_CalculateReconciliationForSingleAllocationInClaimGroupWithMultipleDelGroups()
        {
            var appDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(2,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue:1000.0M,exceptionalAdjustmentsValue:500.0m),
                CreateDeliverable(3,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue:1000.0M,exceptionalAdjustmentsValue:500.0m)
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(10,plannedValue:2000.0M),
                CreateDeliverable(11,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue:1000.0M,exceptionalAdjustmentsValue:500.0m),
                CreateDeliverable(12,actualsToDateValue:3500.0M,forecastRemainingDeliveryValue:1000.0M,exceptionalAdjustmentsValue:500.0m)
            };

            var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0M, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        private FundingClaimReconciliationAllocationGroup CreateAdultSkillsClaimGroup(List<Deliverable> appDeliverables, List<Deliverable> nonappDeliverables)
        {
            var deliverableGroups = new List<DeliverableGroup>
            {
                new DeliverableGroup()
                {
                    DeliverableGroupName = "Apprenticeships",
                    Deliverables = appDeliverables
                },

                new DeliverableGroup()
                {
                    DeliverableGroupName = "Other",
                    Deliverables = nonappDeliverables
                },

            };


            var contractAllocations = new List<ContractAllocation>
            {
                new ContractAllocation() {DeliverableGroups = deliverableGroups}
            };

           return new FundingClaimReconciliationAllocationGroup()
            {
                ReconciliationAllocationGroupName = "AdultSkills1516",
               ClaimTypeName = "MID YEAR",
               ContractAllocations = contractAllocations
            };

        }

        private PolicyConfiguration CreatePolicyConfiguration()
        {
            return new PolicyConfiguration
            {
                DeliverableGroupSettings = new List<DeliverableGroupBehaviour>()
                {
                    new DeliverableGroupBehaviour()
                    {
                        DeliverableGroupName = "Apprenticeships",
                        IsClaimToProfileCappingNeeded = false
                    },
                     new DeliverableGroupBehaviour()
                    {
                        DeliverableGroupName = "Other",
                        IsClaimToProfileCappingNeeded = true
                    },
                     new DeliverableGroupBehaviour()
                    {
                        DeliverableGroupName = "CommunityLearning",
                        IsClaimToProfileCappingNeeded = false
                    },
                    new DeliverableGroupBehaviour()
                    {
                        DeliverableGroupName = "DLS",
                        IsClaimToProfileCappingNeeded = false
                    },
                    new DeliverableGroupBehaviour()
                    {
                        DeliverableGroupName = "24Plus",
                        IsClaimToProfileCappingNeeded = false
                    }

                },
                ClaimGroupSettings = new List<ReconciliationAllocationGroupClaimTypeBehaviour>()
                {
                    new ReconciliationAllocationGroupClaimTypeBehaviour()
                    {
                        ReconciliationAllocationGroupName = "AdultSkills1516",
                        ClaimTypeName = "MID YEAR",
                        OverDeliveryDeminimis = 0.0M,
                        UnderDeliveryDeminimis = -100.0M,
                        ReconcileIfOverDelivery = false,
                        ReconcileIfUnderDelivery = true
                    },
                    new ReconciliationAllocationGroupClaimTypeBehaviour()
                    {
                        ReconciliationAllocationGroupName = "CommunityLearning1516",
                        ClaimTypeName = "MID YEAR",
                        OverDeliveryDeminimis = 0.0M,
                        UnderDeliveryDeminimis = -100.0M,
                        ReconcileIfOverDelivery = false,
                        ReconcileIfUnderDelivery = true
                    },
                    new ReconciliationAllocationGroupClaimTypeBehaviour()
                    {
                        ReconciliationAllocationGroupName = "24PlusAndDLS1516",
                        ClaimTypeName = "MID YEAR",
                        OverDeliveryDeminimis = 100.0M,
                        UnderDeliveryDeminimis = -100.0M,
                        ReconcileIfOverDelivery = true,
                        ReconcileIfUnderDelivery = true
                    }
                }
            };
        }

        #endregion

        #region DLS and 24 Plus (Multiple Allocation each with single del group
        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:2000.0M),
                CreateDeliverable(2),
                CreateDeliverable(3,actualsToDateValue:300.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:250.0m),
                CreateDeliverable(4,actualsToDateValue:500.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:350.0m),
                CreateDeliverable(5),
                CreateDeliverable(6),
                CreateDeliverable(7),
                CreateDeliverable(8),
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:250.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-1000.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-400.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-600.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_1_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_2_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:25.0M,exceptionalAdjustmentsValue:25.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_3_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:150.0M,exceptionalAdjustmentsValue:149.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:25.0M,exceptionalAdjustmentsValue:25.0m),
            };

           
            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-51.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-50.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_4_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:150.0M,exceptionalAdjustmentsValue:100.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };

            

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_5_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:199.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:300.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:100.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };
            
            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-101.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_6_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:150.0M,exceptionalAdjustmentsValue:99.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:51.0m),
            };

            
            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_7_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:100.0m),
                CreateDeliverable(4,actualsToDateValue:30.0M,forecastRemainingDeliveryValue:10.0M,exceptionalAdjustmentsValue:10.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_8_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:10.0M,forecastRemainingDeliveryValue:10.0M,exceptionalAdjustmentsValue:29.0m),
            };

            
            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-101.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_9_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:51.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:30.0M,forecastRemainingDeliveryValue:10.0M,exceptionalAdjustmentsValue:10.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_10_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:400.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_11_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:300.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:151.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:150.0m),
                CreateDeliverable(4,actualsToDateValue:100.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };
            
            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(51.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(50.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_12_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0M),
                CreateDeliverable(3,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:150.0m)
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                CreateDeliverable(1,plannedValue:1000.0m,actualsToDateValue:850.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(2,plannedValue:500.0m,actualsToDateValue:350.0M,forecastRemainingDeliveryValue:100.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(3,plannedValue:500.0m,actualsToDateValue:150.0M,forecastRemainingDeliveryValue:150.0M,exceptionalAdjustmentsValue:50.0m),
                CreateDeliverable(4,actualsToDateValue:50.0M,forecastRemainingDeliveryValue:50.0M,exceptionalAdjustmentsValue:50.0m),
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_13_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 151.0M
                    //ClaimedValue = 601.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                    ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue =350.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 50.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =150.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(101.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_14_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                    ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue =350.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 51.0M
                    //ClaimedValue =251.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(101.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_15_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 300.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                    ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue =350.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                    ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =250.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_16_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 49.0M
                    //ClaimedValue = 299.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                    ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 100.0M
                    //ClaimedValue =350.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 150.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =250.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-101.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_17_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue = 600.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                    ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =250.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 10.0m,
                    ForecastRemainingDeliveryValue = 20.0m,
                    ExceptionalAdjustmentsValue = 20.0M
                    //ClaimedValue =50.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(0.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }


        [TestMethod]
        [TestCategory("DLSAnd24Plus")]
        public void Scenario_18_CalculateReconciliationForMultipleAllocationsInClaimGroupWithSingleDelGroups()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4, ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue = 600.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5
                },
                new Deliverable()
                {
                    DeliverableCode = 6
                },
                new Deliverable()
                {
                    DeliverableCode = 7
                },
                new Deliverable()
                {
                    DeliverableCode = 8
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                     ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =250.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 30.0m,
                    ForecastRemainingDeliveryValue = 10.0m,
                    ExceptionalAdjustmentsValue = 9.0M
                    //ClaimedValue =49.0M
                }
            };

            var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);

            var engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            Assert.AreEqual(-101.0m, claimGroup.ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(0.0m, claimGroup.ContractAllocations[0].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
            Assert.AreEqual(-101.0M, claimGroup.ContractAllocations[1].ReconciledValue.GetValueOrDefault(), "Reconcilialtion Value is incorrect");
        }

        private static FundingClaimReconciliationAllocationGroup CreateDlsAnd24PlusFundingClaimGroup(List<Deliverable> dlsDeliverables,
           List<Deliverable> twentfourPlusDeliverables)
        {
            var dlSdeliverableGroups = new List<DeliverableGroup>
            {
                new DeliverableGroup()
                {
                    DeliverableGroupName = "DLS",
                    Deliverables = dlsDeliverables
                },
            };
            var twentyFourPlusdeliverableGroups = new List<DeliverableGroup>
            {
                new DeliverableGroup()
                {
                    DeliverableGroupName = "24Plus",
                    Deliverables = twentfourPlusDeliverables
                },
            };

            var contractAllocations = new List<ContractAllocation>
            {
                new ContractAllocation() {DeliverableGroups = dlSdeliverableGroups, AllocationReference = "DLS-1234"},
                new ContractAllocation()
                {
                    DeliverableGroups = twentyFourPlusdeliverableGroups,
                    AllocationReference = "ALLBC-1234"
                }
            };

            var claimGroup = new FundingClaimReconciliationAllocationGroup()
            {
                ReconciliationAllocationGroupName = "24PlusAndDLS1516",
                ClaimTypeName = "MID YEAR",
                ContractAllocations = contractAllocations
            };
            return claimGroup;
        }
        #endregion DLS and 24 Plus (Multiple Allocation each with single del group

        #region performance test
        [TestMethod]
        [TestCategory("PerformanceTest")]
        public void CreateOneThousandDlsAnd24PlusFundingClaimGroup()
        {
            var dlsDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    //ClaimedValue = 600.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                    },
                new Deliverable()
                {
                    DeliverableCode = 6,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 7,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 8,
                    ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 150.0M
                },
            };

            var twentfourPlusDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 1000.0M,
                     ActualsToDateValue = 850.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 1000.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 350.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =500.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 3,
                    PlannedValue = 500.0M,
                     ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue =250.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 4,
                     ActualsToDateValue = 30.0m,
                    ForecastRemainingDeliveryValue = 10.0m,
                    ExceptionalAdjustmentsValue = 9.0M
                    //ClaimedValue =49.0M
                }
            };

            var policyConfiguration = CreatePolicyConfiguration();

            for (var i = 0; i <= 1000; i++)
               
            {
                var claimGroup = CreateDlsAnd24PlusFundingClaimGroup(dlsDeliverables, twentfourPlusDeliverables);
                var engine = new ReconciliationCalculationEngine();
                engine.CalculateReconciliation(claimGroup, policyConfiguration);
            }
        }

        [TestMethod]
        [TestCategory("PerformanceTest")]
        public void CreateOneThousandCommunityFundingClaimGroup()
        {
            var policyConfiguration = CreatePolicyConfiguration();
            for (var i = 0; i <= 1000; i++)
            {
                var claimGroup = CreateCommunityLearningFundingClaim(1000.0M, 899.0m,1.0m,210.0m);
                var engine = new ReconciliationCalculationEngine();
                engine.CalculateReconciliation(claimGroup, policyConfiguration);
            }
        }


        [TestCategory("PerformanceTest")]
        [TestMethod]
        public void CreateOneThousandAdultSkillsFundingClaimGroup()
        {
            var appDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 1,
                    PlannedValue = 5000.0M,
                },
                new Deliverable()
                {
                    DeliverableCode = 2,
                      ActualsToDateValue = 50.0m,
                    ForecastRemainingDeliveryValue = 50.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 150.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 3,
                      ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                    //ClaimedValue = 250.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 4,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 5,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 6,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                 new Deliverable()
                {
                    DeliverableCode = 7,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 8,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                 new Deliverable()
                {
                    DeliverableCode = 9
                }
            };

            var nonappDeliverables = new List<Deliverable>()
            {
                new Deliverable()
                {
                    DeliverableCode = 10,
                    PlannedValue = 2500.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 11,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 90.0M
                    //ClaimedValue =290.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 12,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 70.0M
                    //ClaimedValue =270.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 13,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 14,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                }
                ,
                new Deliverable()
                {
                    DeliverableCode = 15,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 16,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 17,
                       ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                },
                new Deliverable()
                {
                    DeliverableCode = 18,
                    ActualsToDateValue = 100.0m,
                    ForecastRemainingDeliveryValue = 100.0m,
                    ExceptionalAdjustmentsValue = 50.0M
                }
            };

            var policyConfiguration = CreatePolicyConfiguration();
            for (var i = 0; i <= 1000; i++)
            {
                var claimGroup = CreateAdultSkillsClaimGroup(appDeliverables, nonappDeliverables);
                var engine = new ReconciliationCalculationEngine();
                engine.CalculateReconciliation(claimGroup, policyConfiguration);
            }
        }

        #endregion performance test        

       
  }
}
