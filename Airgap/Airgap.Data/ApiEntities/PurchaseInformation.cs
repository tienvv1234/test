using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.ApiEntities
{
    public class PurchaseInformation
    {
        public string PlanId { get; set; }
        public string NameOnCard { get; set; }
        public string CC_number { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string CCVCode { get; set; }
        public int CurrenUserId { get; set; }
        public int CurrenApplianceId { get; set; }
        public string Coupon { get; set; }
    }
}
