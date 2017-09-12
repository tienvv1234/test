using Airgap.Entity;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airgap.Service
{
    public class NotificationService : INotificationService
    {
        private IRepository<Notification> repoNotification;
        public NotificationService(IRepository<Notification> repoNotification)
        {
            this.repoNotification = repoNotification;
        }

        public Notification Insert(Notification notification)
        {
            return repoNotification.Insert(notification);
        }

        public void RemoveNotificationByApplianceId(long applianceId)
        {
            var lNotification = repoNotification.FindAll(x => x.ApplianceId == applianceId);
            foreach (var item in lNotification.ToList())
            {
                repoNotification.Delete(item);
            }
        }
    }
}
