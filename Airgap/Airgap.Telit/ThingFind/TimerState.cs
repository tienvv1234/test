using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingFind
{
    public class TimerState
    {
        public string ts { get; set; }
        public string since { get; set; }
        public int state { get; set; }
        public string msg { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
