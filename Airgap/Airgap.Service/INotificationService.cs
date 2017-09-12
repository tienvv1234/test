using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface INotificationService
    {
        void RemoveNotificationByApplianceId(long applianceId);
        Notification Insert(Notification notification);
    }
}
