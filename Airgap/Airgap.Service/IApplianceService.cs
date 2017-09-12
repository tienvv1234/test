using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IApplianceService
    {
        List<Appliance> GetApplianceByAccountId(long id);
        Appliance GetApplianceById(long id);
        Appliance GetApplianceBySerialNumber(string serialNumber);
        Appliance GetApplianceBySubscriptionId(string subscriptionId);
        Appliance Update(Appliance appliance);
        Appliance Insert(Appliance appliance);
    }
}
