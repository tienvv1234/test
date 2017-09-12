using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Airgap.Telit.ThingFind
{
    [DataContract]
    public class Attrs
    {
        [DataMember(Name = "name")]
        public Name name { get; set; }
        [DataMember(Name = "timer-schedule")]
        public TimerSchedule timerSchedule { get; set; }
    }
}
