using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Airgap.Telit.ThingFind
{
    [DataContract]
    public class Alarms
    {
        [DataMember(Name = "env")]
        public Env env { get; set; }
        [DataMember(Name = "on")]
        public On on { get; set; }
        [DataMember(Name = "timer-state")]
        public TimerState timerState { get; set; }
        [DataMember(Name = "trust")]
        public Trust trust { get; set; }
    }
}
