using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingList
{
    public class Loc
    {
        public Addr addr { get; set; }
        public string fixType { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int speed { get; set; }
    }
}
