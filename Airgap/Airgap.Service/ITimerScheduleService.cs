using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface ITimerScheduleService
    {
        List<TimerScheduleDTO> GetTimerScheduleDTOByApplianceId(long id);
        List<TimerSchedule> GetTimerScheduleByApplianceId(long id);

        TimerSchedule Insert(TimerSchedule timerSchedule);
        TimerSchedule Update(TimerSchedule timerSchedule);
    }
}
