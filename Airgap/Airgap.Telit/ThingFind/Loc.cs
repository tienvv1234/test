using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingFind
{
    public class Loc
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public string fixType { get; set; }
        public int speed { get; set; }
        public Addr addr { get; set; }
    }
}
