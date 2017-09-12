using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingFind
{
    public class Params
    {
        public string id { get; set; }
        public string orgId { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string defId { get; set; }
        public string defName { get; set; }
        public string imei { get; set; }
        public List<string> tags { get; set; }
        public string lastSeen { get; set; }
        public string lastCommunication { get; set; }
        public bool locEnabled { get; set; }
        public string locUpdated { get; set; }
        public Loc loc { get; set; }
        public Attrs attrs { get; set; }
        public string tunnelActualHost { get; set; }
        public string tunnelVirtualHost { get; set; }
        public Alarms alarms { get; set; }
        public int storage { get; set; }
        public string createdBy { get; set; }
        public string createdOn { get; set; }
        public string updatedBy { get; set; }
        public string updatedOn { get; set; }
        public string defKey { get; set; }
        public bool connected { get; set; }
        public bool appId { get; set; }
        public string iccid { get; set; }
    }
}
