using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconciliationCalculator
{
    public class Deliverable
    {
        public int DeliverableCode { get; set; }
        public decimal? PlannedValue { get; set; }
        public decimal? ActualsToDateValue { get; set; }
        public decimal? ForecastRemainingDeliveryValue { get; set; }
        public decimal? ExceptionalAdjustmentsValue { get; set; }
        public decimal? ClaimedValue { get; set; }
        public decimal? VarianceValue { get; set; }
    }
}
