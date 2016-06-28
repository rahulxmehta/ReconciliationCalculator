using ReconciliationCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ReconciliationAPI.Controllers
{
    public class ReconciliationController : ApiController
    {
        [Route("api/Reconciliation")]
        [HttpPost]
        public FundingClaimReconciliationAllocationGroup CalculateReconciliation([FromBody]FundingClaimReconciliationAllocationGroup claimGroup)
        {
            var  engine = new ReconciliationCalculationEngine();
            engine.CalculateReconciliation(claimGroup, CreatePolicyConfiguration());
            //engine.CalculateReconciliation(claimGroup, policyConfiguration);
            return claimGroup;
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
    }
}
