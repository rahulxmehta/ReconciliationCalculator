Feature: CalculateReconciliationForMultipleAllocationEachHavingSingleDeliverableGroup

@MultipleAllocationSingleDelGroup
Scenario: Calculate Reconciliation for a funding claim group that has multiple allocation each having single deliverable group 
	Given A funding claim reconciliation allocation group exists with name 'DLSAnd24PlusBC1516' And Claim Type as 'MID YEAR' that contains Allocation as follows
	| Allocation Reference |
	| ALLBC-1234           |
	| DLS-1234             |

	And the Allocation Reference 'ALLBC-1234' contains Deliverable Group '24PlusBC' that contains following deliverables
	| Deliverable Code | Deliverable Name                                  | Deliverable Planned Value | Deliverable Actuals to Date Value | Deliverable Forecast Remaining Delivery Value | Deliverable Exceptional Adjustments Value |
	| 1                | Total 24+ Advanced Learning Loans Bursary Funding | 2000.00                   | 0.00                              | 0.00                                          | 0.00                                      |
	| 2                | 24+ Loans - Bursary Funding                       | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 3                | 24+ Loans - Area Costs                            | 0.00                      | 100.00                            | 500.00                                        | 50.00                                     |
	| 4                | 24+ Loans - Excess Claims                         | 0.00                      | 250.00                            | 500.00                                        | 200.00                                    |
	| 5                | 24+ Loans - Hardship                              | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 6                | 24+ Loans - Childcare                             | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 7                | 24+ Loans - Residential Access Fund               | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      |
	| 8                | 24+ Loans - Administration Expenditure            | 0.00                      | 0.00                              | 0.00                                          | 0.00                                      | 
	
	And the Allocation Reference 'DLS-1234' contains Deliverable Group 'DLS' that contains following deliverables
	| Deliverable Code | Deliverable Name           | Deliverable Planned Value | Deliverable Actuals to Date Value | Deliverable Forecast Remaining Delivery Value | Deliverable Exceptional Adjustments Value |
	| 1                | 19+ Hardship               | 1000.00                   | 300.00                            | 0.00                                          | 400.00                                    |
	| 2                | 20+ Childcare              | 500.00                    | 100.00                            | 0.00                                          | 50.00                                     |
	| 3                | Residential Access Fund    | 500.00                    | 300.00                            | 0.00                                          | 50.00                                     |
	| 4                | Administration Expenditure | 0.00                      | 100.00                            | 0.00                                          | 100.00                                    |
	
	And Policy Configuration Exists for Reconciliation Allocation Group 'DLSAnd24PlusBC1516' And Claim Type Name as 'MID YEAR' with behaviour as :
	| ReconcileOverDelivery | OverDeliveryDeminimis | ReconcileUnderDelivery | UnderDeliveryDeminimis |
	| True                  | 100.00                | True                   | -100.00                |

	And Deliverable Group '24PlusBC' exists as follows
	| IsClaimToProfileCappingNeeded |
	| False                         |
	
	And Deliverable Group 'DLS' exists as follows
	| IsClaimToProfileCappingNeeded |
	| False                         |

	When reconciliation is calculated

	Then the result should be at level of funding claim group
	| Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| 4000.00       | 3000.00       | 3000.00              | -1000.00                      | -1000.00             |
	And the result at the Allocation level should be 
	| Allocation Reference | Planned Value | Claimed Value | Capped Claimed Value | Proposed Reconciliation Value | Reconciliation Value |
	| ALLBC-1234           | 2000.00       | 1600.00       | 1600.00              | -400.00                       | -400.00              |
	| DLS-1234             | 2000.00       | 1400.00       | 1400.00              | -600.00                       | -600.00              |