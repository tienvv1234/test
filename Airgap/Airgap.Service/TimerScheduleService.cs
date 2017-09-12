using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;
using Airgap.Data.DTOEntities;

namespace Airgap.Service
{
    public class TimerScheduleService : ITimerScheduleService
    {
        private IRepository<TimerSchedule> repoTimerSchedule;
        private IRepository<TimerType> repoTimerType;

        public TimerScheduleService(IRepository<TimerSchedule> repoTimerSchedule, IRepository<TimerType> repoTimerType)
        {
            this.repoTimerSchedule = repoTimerSchedule;
            this.repoTimerType = repoTimerType;
        }

        public List<TimerSchedule> GetTimerScheduleByApplianceId(long id)
        {
            return repoTimerSchedule.FindAll(x => x.ApplianceId == id).ToList();
        }

        public List<TimerScheduleDTO> GetTimerScheduleDTOByApplianceId(long id)
        {
            var result = (from tc in repoTimerSchedule.Table
                           join tt in repoTimerType.Table on tc.TimerTypeId equals tt.Id
                           where tc.ApplianceId == id
                           select new
                           {
                               timerSchedule = tc,
                               timerType = tt
                           }).AsEnumerable().Select(a => new TimerScheduleDTO
                           {
                               ActiveValues = a.timerSchedule.ActiveValues,
                               ApplianceId = a.timerSchedule.ApplianceId,
                               TimerTypeId = a.timerSchedule.TimerTypeId,
                               TimerTypeName = a.timerType.TimerTypeName
                           });

            return result.ToList();
        }

        public TimerSchedule Insert(TimerSchedule timerSchedule)
        {
            return repoTimerSchedule.Insert(timerSchedule);
        }

        public TimerSchedule Update(TimerSchedule timerSchedule)
        {
            return repoTimerSchedule.Update(timerSchedule);
        }
    }
    
}
