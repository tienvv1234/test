using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airgap.Service
{
    public interface IAccountApplianceService
    {
        List<AccountAppliance> GetAccountApplianceByAccountId(long id);
        List<AccountDTO> GetAccountByApplianceId(long id, bool? isVerified);
        List<ApplianceDTO> GetApplianceByAccountId(long id);
        AccountAppliance Insert(AccountAppliance accountAppliance);
        AccountAppliance Update(AccountAppliance accountAppliance);
        void Remote(long applianceId);
        void RemoteUser(long accountId, long applianceId);
        List<AccountAppliance> GetAccountApplianceByUUID(string uuId);
        Account GetAccountByUUID(string uuId);
        List<ApplianceDTO> GetApplianceByUUID(string uuId);
        List<ApplianceDTO> GetApplianceBySubscriptionId(string subscriptionId);
        AccountAppliance GetAccountApplianceByAccountIdAndApplianceId(long accountId, long applianceId);
        List<AccountAppliance> GetAccountApplianceBySubScription(string subscriptionId);
        List<ApplianceDTO> GetAllApplianceOfUUID(string uuid);
        List<AccountAppliance> GetAccountApplianceByAccountIdForUpdate(long id);
    }
}
