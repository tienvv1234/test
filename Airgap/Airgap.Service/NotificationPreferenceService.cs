using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;

namespace Airgap.Service
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private IRepository<NotificationPreference> repoNotificationPreference;

        public NotificationPreferenceService(IRepository<NotificationPreference> repoNotificationPreference)
        {
            this.repoNotificationPreference = repoNotificationPreference;
        }

        public List<NotificationPreference> GetNotificationPreferenceByAccountId(long accountId)
        {
            return repoNotificationPreference.FindAll(x => x.AccountId == accountId).ToList();
        }

        public List<NotificationPreference> GetNotificationPreferenceByApplianceId(long applianceId)
        {
            return repoNotificationPreference.FindAll(x => x.ApplianceId == applianceId).ToList();
        }

        public NotificationPreference Insert(NotificationPreference notificationPreference)
        {
            return repoNotificationPreference.Insert(notificationPreference);
        }

        public bool Remove(long accountId, long applianceId, long timerTypeId)
        {
            var notificationPreference = repoNotificationPreference.Find(x => x.AccountId == accountId && x.ApplianceId == applianceId && x.EventTypeId == timerTypeId);
            return repoNotificationPreference.Delete(notificationPreference);
        }

        public void RemoveNotificationPreferenceByApplianceId(long applianceId)
        {
            var notificationPreference = repoNotificationPreference.FindAll(x => x.ApplianceId == applianceId);
            foreach (var item in notificationPreference.ToList())
            {
                repoNotificationPreference.Delete(item);
            }
        }
    }
}
