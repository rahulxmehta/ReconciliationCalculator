Feature: CalculateReconciliationForSingleAllocationHavingMultipleDeliverableGroup

	@SingleAllocationMultipleDelGroup
Scenario: Calculate Reconciliation for a funding claim group that has single allocation which in turn has a multiple deliverable group 
	Given A funding claim reconciliation allocation group exists with name 'AdultSkills1516' And Claim Type as 'MID YEAR' that contains Allocation as follows
	| Allocation Reference | 
	| ASC-1234             | 

	And the Allocation Reference 'ASC-1234' contains Deliverable Group 'Apprenticeships' that contains following deliverables
	| Deliverable Code | Deliverable Name                                    | Deliverable Planned Value | Deliverable Actuals to Date Value | Deliverable Forecast Remaining Delivery Value | Deliverable Exceptional Adjustments Value |
	| 1                | Apprenticeships Total Funding                       | 1000.00                   | 0.00                              | 0.00                                          | 0.00                                      |
	| 2                | 19-23 Apprenticeships Programme Funding             | 0.00                      | 100.00                            | 50.00                                         | 0.00                                      |
	| 3                | 19-23 Apprenticeships Learning Support              | 0.00                      | 250.00                            | 0.00                                          | 0.00                                      |
	| 4                | 19+ Apprenticeships Learner Support                 | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 5                | 24+ Apprenticeships Programme Funding               | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 6                | 24+ Apprenticeships Learning Support                | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 7                | 19-23 Trailblazer Apprenticeships Programme Funding | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 8                | 19-23 Trailblazer Apprenticeships Learning Support  | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 9                | 24+ Trailblazer Apprenticeships Programme Funding   | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 10               | 24+ Trailblazer Apprenticeships Learning Support    | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	
	And the Allocation Reference 'ASC-1234' contains Deliverable Group 'OtherASB' that contains following deliverables
	| Deliverable Code | Deliverable Name                                    | Deliverable Planned Value | Deliverable Actuals to Date Value | Deliverable Forecast Remaining Delivery Value | Deliverable Exceptional Adjustments Value |
	| 11               | Other ASB Total Funding                             | 500.00                    | 0.00                              | 0.00                                          | 0.00                                      |
	| 12               | Classroom Learning Programme Funding                | 0.00                      | 300.00                            | 0.00                                          | -10.00                                    |
	| 13               | Classroom Learning - Learning Support               | 0.00                      | 270.00                            | 0.00                                          | 0.00                                      |
	| 14               | 19-24 Traineeships Programme Funding                | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 15               | 19-24 Traineeships Learning Support                 | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 16               | 19-24 Traineeships Learner Support                  | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 17               | 19-23 Trailblazer Apprenticeships Programme Funding | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 18               | Workplace Learning Programme Funding                | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 19               | Workplace Learning - Learning Support               | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
		
	And Policy Configuration Exists for Reconciliation Allocation Group 'AdultSkills1516' And Claim Type Name as 'MID YEAR' with behaviour as :
	| ReconcileOverDelivery | OverDeliveryDeminimis | ReconcileUnderDelivery | UnderDeliveryDeminimis |
	| False                 | 0.00                  | True                   | -100.00                |
	
	And Deliverable Group 'Apprenticeships' exists as follows
	| IsClaimToProfileCappingNeeded |
	| False                         |

	And Deliverable Group 'OtherASB' exists as follows
	| IsClaimToProfileCappingNeeded |
	| True                          |
	
	When reconciliation is calculated
	
	Then the result should be at level of funding claim group
	| Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| 1500.00       | 960.00        | 900.00               | -600.00                       | -600.00              |
	And the result at the Allocation level should be 
	| Allocation Reference | Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| ASC-1234             | 1500.00       | 960.00        | 900.00               | -600.00                       | -600.00              |