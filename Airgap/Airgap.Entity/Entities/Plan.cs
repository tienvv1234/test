using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Entity.Entities
{
    public class Plan
    {
        public Plan()
        {   
        }

        public Plan(StripePlan plan)
        {
            this.PlanId = plan.Id;
            this.Amount = plan.Amount / 100;
            this.Created = plan.Created;
            this.Currency = plan.Currency;
            this.Interval = plan.Interval;
            this.IntervalCount = plan.IntervalCount;
            this.Name = plan.Name;
            this.TrialPeriodDays = plan.TrialPeriodDays;
        }

        public int Id { get; set; }
        public string PlanId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public decimal Amount { get; set; }
        public string Interval { get; set; }
        public int IntervalCount { get; set; }
        public int? TrialPeriodDays { get; set; }
    }
}
