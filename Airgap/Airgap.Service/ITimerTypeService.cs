using Airgap.Entity;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airgap.Service
{
    public interface ITimerTypeService
    {
        TimerType Insert(TimerType timerType);
        TimerType Update(TimerType timerType);
        List<TimerType> GetTimers();
    }
}
