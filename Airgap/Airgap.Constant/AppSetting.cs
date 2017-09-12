using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airgap.Constant
{
    public class AppSetting
    {
        public string ApiUrl { get; set; }
        public string TelitHttpUrl { get; set; }
        public string TelitHttpsUrl { get; set; }
        public string MailHost { get; set; }
        public string MailUsername { get; set; }
        public string MailPassword { get; set; }
        public string From { get; set; }
        public string Salt { get; set; }
        public string APIkey { get; set; }
        public string TelitUsername { get; set; }
        public string TelitPassword { get; set; }
        public string GCMPushNotification { get; set; }
        public string APIOfGCM { get; set; }
        public string AppleHost { get; set; }
        public string APIKeyOfStripe { get; set; }
        public string Miles { get; set; }
        public string TriggerMiles { get; set; }
    }
}
