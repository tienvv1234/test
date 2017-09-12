using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class TimerScheduleDTO
    {
        public long? ApplianceId { get; set; }

        public long? TimerTypeId { get; set; }

        public string ActiveValues { get; set; }

        public string TimerTypeName { get; set; }
    }
}
