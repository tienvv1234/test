using Airgap.Entity;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airgap.Service
{
    public class TimerTypeService: ITimerTypeService
    {
        private IRepository<TimerType> repoTimerType;
        public TimerTypeService(IRepository<TimerType> repoTimerType)
        {
            this.repoTimerType = repoTimerType;
        }
        public TimerType Insert(TimerType timerType)
        {
            return repoTimerType.Insert(timerType);
        }

        public TimerType Update(TimerType timerType)
        {
            return repoTimerType.Update(timerType);
        }
        public List<TimerType> GetTimers()
        {
            return repoTimerType.GetAll().ToList();
        }
    }
}
