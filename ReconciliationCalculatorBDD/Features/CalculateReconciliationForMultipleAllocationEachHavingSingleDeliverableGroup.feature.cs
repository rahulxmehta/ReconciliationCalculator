﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.1.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ReconciliationCalculatorBDD.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("CalculateReconciliationForMultipleAllocationEachHavingSingleDeliverableGroup")]
    public partial class CalculateReconciliationForMultipleAllocationEachHavingSingleDeliverableGroupFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "CalculateReconciliationForMultipleAllocationEachHavingSingleDeliverableGroup.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CalculateReconciliationForMultipleAllocationEachHavingSingleDeliverableGroup", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Calculate Reconciliation for a funding claim group that has multiple allocation e" +
            "ach having single deliverable group")]
        [NUnit.Framework.CategoryAttribute("MultipleAllocationSingleDelGroup")]
        public virtual void CalculateReconciliationForAFundingClaimGroupThatHasMultipleAllocationEachHavingSingleDeliverableGroup()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Calculate Reconciliation for a funding claim group that has multiple allocation e" +
                    "ach having single deliverable group", new string[] {
                        "MultipleAllocationSingleDelGroup"});
#line 4
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Allocation Reference"});
            table1.AddRow(new string[] {
                        "ALLBC-1234"});
            table1.AddRow(new string[] {
                        "DLS-1234"});
#line 5
 testRunner.Given("A funding claim reconciliation allocation group exists with name \'DLSAnd24PlusBC1" +
                    "516\' And Claim Type as \'MID YEAR\' that contains Allocation as follows", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Deliverable Code",
                        "Deliverable Name",
                        "Deliverable Planned Value",
                        "Deliverable Actuals to Date Value",
                        "Deliverable Forecast Remaining Delivery Value",
                        "Deliverable Exceptional Adjustments Value"});
            table2.AddRow(new string[] {
                        "1",
                        "Total 24+ Advanced Learning Loans Bursary Funding",
                        "2000.00",
                        "0.00",
                        "0.00",
                        "0.00"});
            table2.AddRow(new string[] {
                        "2",
                        "24+ Loans - Bursary Funding",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00"});
            table2.AddRow(new string[] {
                        "3",
                        "24+ Loans - Area Costs",
                        "0.00",
                        "100.00",
                        "500.00",
                        "50.00"});
            table2.AddRow(new string[] {
                        "4",
                        "24+ Loans - Excess Claims",
                        "0.00",
                        "250.00",
                        "500.00",
                        "200.00"});
            table2.AddRow(new string[] {
                        "5",
                        "24+ Loans - Hardship",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00"});
            table2.AddRow(new string[] {
                        "6",
                        "24+ Loans - Childcare",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00"});
            table2.AddRow(new string[] {
                        "7",
                        "24+ Loans - Residential Access Fund",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00"});
            table2.AddRow(new string[] {
                        "8",
                        "24+ Loans - Administration Expenditure",
                        "0.00",
                        "0.00",
                        "0.00",
                        "0.00"});
#line 10
 testRunner.And("the Allocation Reference \'ALLBC-1234\' contains Deliverable Group \'24PlusBC\' that " +
                    "contains following deliverables", ((string)(null)), table2, "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Deliverable Code",
                        "Deliverable Name",
                        "Deliverable Planned Value",
                        "Deliverable Actuals to Date Value",
                        "Deliverable Forecast Remaining Delivery Value",
                        "Deliverable Exceptional Adjustments Value"});
            table3.AddRow(new string[] {
                        "1",
                        "19+ Hardship",
                        "1000.00",
                        "300.00",
                        "0.00",
                        "400.00"});
            table3.AddRow(new string[] {
                        "2",
                        "20+ Childcare",
                        "500.00",
                        "100.00",
                        "0.00",
                        "50.00"});
            table3.AddRow(new string[] {
                        "3",
                        "Residential Access Fund",
                        "500.00",
                        "300.00",
                        "0.00",
                        "50.00"});
            table3.AddRow(new string[] {
                        "4",
                        "Administration Expenditure",
                        "0.00",
                        "100.00",
                        "0.00",
                        "100.00"});
#line 21
 testRunner.And("the Allocation Reference \'DLS-1234\' contains Deliverable Group \'DLS\' that contain" +
                    "s following deliverables", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "ReconcileOverDelivery",
                        "OverDeliveryDeminimis",
                        "ReconcileUnderDelivery",
                        "UnderDeliveryDeminimis"});
            table4.AddRow(new string[] {
                        "True",
                        "100.00",
                        "True",
                        "-100.00"});
#line 28
 testRunner.And("Policy Configuration Exists for Reconciliation Allocation Group \'DLSAnd24PlusBC15" +
                    "16\' And Claim Type Name as \'MID YEAR\' with behaviour as :", ((string)(null)), table4, "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "IsClaimToProfileCappingNeeded"});
            table5.AddRow(new string[] {
                        "False"});
#line 32
 testRunner.And("Deliverable Group \'24PlusBC\' exists as follows", ((string)(null)), table5, "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "IsClaimToProfileCappingNeeded"});
            table6.AddRow(new string[] {
                        "False"});
#line 36
 testRunner.And("Deliverable Group \'DLS\' exists as follows", ((string)(null)), table6, "And ");
#line 40
 testRunner.When("reconciliation is calculated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Planned Value",
                        "Claimed Value",
                        "Capped Claimed Value",
                        "Proposed Reconciliation Value",
                        "Reconciliation Value"});
            table7.AddRow(new string[] {
                        "4000.00",
                        "3000.00",
                        "3000.00",
                        "-1000.00",
                        "-1000.00"});
#line 42
 testRunner.Then("the result should be at level of funding claim group", ((string)(null)), table7, "Then ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Allocation Reference",
                        "Planned Value",
                        "Claimed Value",
                        "Capped Claimed Value",
                        "Proposed Reconciliation Value",
                        "Reconciliation Value"});
            table8.AddRow(new string[] {
                        "ALLBC-1234",
                        "2000.00",
                        "1600.00",
                        "1600.00",
                        "-400.00",
                        "-400.00"});
            table8.AddRow(new string[] {
                        "DLS-1234",
                        "2000.00",
                        "1400.00",
                        "1400.00",
                        "-600.00",
                        "-600.00"});
#line 45
 testRunner.And("the result at the Allocation level should be", ((string)(null)), table8, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion