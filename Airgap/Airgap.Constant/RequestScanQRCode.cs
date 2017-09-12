using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Constant
{
    public class RequestScanQRCode
    {
        public string serialnumber { get; set; }
        public string uuid { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string phoneType { get; set; }
        public string devicetoken { get; set; }
        public string devicename { get; set; }
        public string phonenumber { get; set; }
    }
}
