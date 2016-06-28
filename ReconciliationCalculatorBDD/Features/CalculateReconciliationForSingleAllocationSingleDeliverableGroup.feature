Feature: CalculateReconciliationForSingleAllocationSingleDeliverableGroup

@SingleAllocationSingleDelGroup
Scenario: Calculate Reconciliation for a funding claim group that has single allocation which in turn has a single deliverable group with Variance GT Deminimus
	Given A funding claim reconciliation allocation group exists with name 'CommunityLearning1516' And Claim Type as 'MID YEAR' that contains Allocation as follows
	| Allocation Reference |
	| CL-1234             |
	And  the Allocation Reference 'CL-1234' contains Deliverable Group 'CommunityLearning' that contains following deliverables
	| Deliverable Code | Deliverable Name   | Deliverable Planned Value | Deliverable Actuals to Date Value | Deliverable Forecast Remaining Delivery Value | Deliverable Exceptional Adjustments Value |
	| 1                | Community Learning | 1000.00                   | 800.00                            | 50.00                                         | 0.00                                      | 
	And Policy Configuration Exists for Reconciliation Allocation Group 'CommunityLearning1516' And Claim Type Name as 'MID YEAR' with behaviour as :
	| ReconcileOverDelivery | OverDeliveryDeminimis | ReconcileUnderDelivery | UnderDeliveryDeminimis |
	| False                 | 0.00                  | True                   | -100.00                |
	
	And Deliverable Group 'CommunityLearning' exists as follows
	| IsClaimToProfileCappingNeeded |
	| False                         |
	
	When reconciliation is calculated
	Then the result should be at level of funding claim group
	| Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| 1000.00       | 850.00        | 850.00               | -150.00                       | -150.00              |
	And the result at the Allocation level should be 
	| Allocation Reference | Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| CL-1234              | 1000.00       | 850.00        | 850.00               | -150.00                       | -150.00              | 