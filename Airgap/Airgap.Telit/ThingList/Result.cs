using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingList
{
    public class Result
    {
        public Alarms alarms { get; set; }
        public ApiCounts apiCounts { get; set; }
        public Attrs attrs { get; set; }
        public string billingPlanCode { get; set; }
        public bool connected { get; set; }
        public string createdOn { get; set; }
        public string defKey { get; set; }
        public string defName { get; set; }
        public string iccid { get; set; }
        public string id { get; set; }
        public string imei { get; set; }
        public string key { get; set; }
        public string lastCommunication { get; set; }
        public string lastSeen { get; set; }
        public Loc loc { get; set; }
        public string locUpdated { get; set; }
        public string meid { get; set; }
        public string name { get; set; }
        public Properties properties { get; set; }
        public int storage { get; set; }
        public string updatedOn { get; set; }
        public string vasPackageCode { get; set; }
    }
}
