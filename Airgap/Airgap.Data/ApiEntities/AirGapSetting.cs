using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.ApiEntities
{
    public class AirGapSetting
    {
        public List<TimerScheduleDTO> TimerScheduleDTO { get; set; }

        public List<ApplianceDTO> ListApplianceDTO { get; set; }

        public List<AccountDTO> ListAccountDTO { get; set; }

        public List<State> States { get; set; }

        public bool? IsMatching { get; set; }
    }
}
