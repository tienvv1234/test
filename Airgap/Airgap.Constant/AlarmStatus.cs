using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Constant
{
    public enum AlarmStatus
    {
        OffUnknow = 0,
        OnUnknow = 1,
        OffAppliance = 2,
        OnAppliance = 3,
        OffScheduleTimer = 4,
        OnScheduleTimer = 5,
        OffReserved = 6,
        OnReserved = 7,
        OffPortal = 8,
        OnPortal = 9,
        OffGeofence = 10,
        OnGeofence = 11,
        OffMobile = 12,
        OnMobile = 13,

    }
}
