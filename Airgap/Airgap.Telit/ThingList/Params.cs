using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.ThingList
{
    public class Params
    {
        public int count { get; set; }
        public List<string> fields { get; set; }
        public List<Result> result { get; set; }
    }
}
