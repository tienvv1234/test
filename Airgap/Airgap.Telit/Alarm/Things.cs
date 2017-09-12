using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Telit.Alarm
{
    public class Things
    {
        public bool success { get; set; }
        public List<string> errorMessages { get; set; }
        public List<int> errorCodes { get; set; }
    }
}
