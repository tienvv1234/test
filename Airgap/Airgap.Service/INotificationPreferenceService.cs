using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface INotificationPreferenceService
    {
        List<NotificationPreference> GetNotificationPreferenceByApplianceId(long applianceId);
        List<NotificationPreference> GetNotificationPreferenceByAccountId(long accountId);
        NotificationPreference Insert(NotificationPreference notificationPreference);
        bool Remove(long accountId, long applianceId, long timerTypeId);
        void RemoveNotificationPreferenceByApplianceId(long applianceId);
    }
}
