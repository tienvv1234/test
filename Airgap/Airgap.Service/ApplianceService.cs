using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;
namespace Airgap.Service
{
    public class ApplianceService : IApplianceService
    {
        private IRepository<Appliance> repoAppliance;


        public ApplianceService(IRepository<Appliance> repoAppliance)
        {
            this.repoAppliance = repoAppliance;
        }

        public List<Appliance> GetApplianceByAccountId(long id)
        {
            return repoAppliance.FindAll(x => x.AccountId == id).ToList();
        }

        public Appliance GetApplianceBySerialNumber(string serialNumber)
        {
            return repoAppliance.Find(x => x.SerialNumber == serialNumber);
        }

        public Appliance GetApplianceById(long id)
        {
            return repoAppliance.Find(x => x.Id == id);
        }

        public Appliance Insert(Appliance appliance)
        {
            return repoAppliance.Insert(appliance);
        }

        public Appliance Update(Appliance appliance)
        {
            return repoAppliance.Update(appliance);
        }

        public Appliance GetApplianceBySubscriptionId(string subscriptionId)
        {
            throw new NotImplementedException();
        }
    }
}
