using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;
using Airgap.Data.DTOEntities;

namespace Airgap.Service
{
    public class AccountApplianceService : IAccountApplianceService
    {
        private IRepository<AccountAppliance> repoAccountAppliance;
        private IRepository<Account> repoAccount;
        private IRepository<Appliance> repoAppliance;
        public AccountApplianceService(IRepository<AccountAppliance> repoAccountAppliance, IRepository<Account> repoAccount, IRepository<Appliance> repoAppliance)
        {
            this.repoAccountAppliance = repoAccountAppliance;
            this.repoAccount = repoAccount;
            this.repoAppliance = repoAppliance;
        }

        public List<AccountAppliance> GetAccountApplianceByAccountId(long id)
        {
            var result = (from accapp in repoAccountAppliance.Table
                          join app in repoAppliance.Table on accapp.ApplianceId equals app.Id
                          where accapp.AccountId == id 
                          //hoi lai man hinh chua verify && accapp.IsQRCodeScaned == true && accapp.IsVerified == true
                          select new {
                              appliance = app,
                              accountAppliance = accapp
                          }).AsEnumerable().Select(a => new AccountAppliance
                          {
                              AccountId = a.accountAppliance.AccountId,
                              ApplianceId = a.accountAppliance.ApplianceId,
                              Appliance = a.appliance,
                              AirGapVersion = a.accountAppliance.AirGapVersion,
                              IsQRCodeScaned = a.accountAppliance.IsQRCodeScaned,
                              IsVerified = a.accountAppliance.IsVerified,
                              Lat = a.accountAppliance.Lat,
                              Lon = a.accountAppliance.Lon,
                              DeviceName = a.accountAppliance.DeviceName,
                              DeviceToken = a.accountAppliance.DeviceToken,
                              IdentifierForVendor = a.accountAppliance.IdentifierForVendor,
                              PhoneType = a.accountAppliance.PhoneType,
                              SubscriptionId = a.accountAppliance.SubscriptionId
                          });

            return result.ToList();
        }

        public List<AccountAppliance> GetAccountApplianceByAccountIdForUpdate(long id)
        {
            return repoAccountAppliance.FindAll(x => x.AccountId == id).ToList();
        }

        public List<ApplianceDTO> GetApplianceByUUID(string uuId)
        {
            var resutlt = (from accapp in repoAccountAppliance.Table
                           join app in repoAppliance.Table on accapp.ApplianceId equals app.Id
                           where accapp.IdentifierForVendor == uuId
                           && accapp.IsQRCodeScaned == true && accapp.IsVerified == true
                           select new
                           {
                               appliance = app,
                               accountAppliance = accapp
                           }).AsEnumerable().Select(a => new ApplianceDTO()
                           {
                               AccountId = a.appliance.AccountId,
                               Cellular = a.appliance.Cellular,
                               City = a.appliance.City,
                               DeviceName = a.appliance.DeviceName,
                               GeoFenceEnabled = a.appliance.GeoFenceEnabled,
                               Id = a.appliance.Id,
                               IsConnected = a.appliance.IsConnected,
                               Lat = a.appliance.Lat,
                               Lon = a.appliance.Lon,
                               OsVersion = a.appliance.OsVersion,
                               Power = a.appliance.Power,
                               SerialNumber = a.appliance.SerialNumber,
                               StateId = a.appliance.StateId,
                               Street1 = a.appliance.Street1,
                               Street2 = a.appliance.Street2,
                               TimerEnabled = a.appliance.TimerEnabled,
                               TriggerMile = a.appliance.TriggerMile,
                               TrustLevel = a.appliance.TrustLevel,
                               Wifi = a.appliance.Wifi,
                               WiFiInternet = a.appliance.WiFiInternet,
                               ZipCode = a.appliance.ZipCode,
                               AirGapVersion = a.accountAppliance.AirGapVersion,
                               IsOn = a.appliance.IsOn,
                               Status = a.appliance.Status
                           });

            return resutlt.ToList();
        }

        public List<AccountDTO> GetAccountByApplianceId(long id, bool? isVerified)
        {
            var resutlt = (from accapp in repoAccountAppliance.Table
                           join acc in repoAccount.Table on accapp.AccountId equals acc.Id
                           where accapp.ApplianceId == id 
                           && accapp.IsQRCodeScaned == true 
                           && (isVerified == null || accapp.IsVerified == isVerified)
                           select new
                           {
                               account = acc,
                               accountAppliance = accapp
                           }).AsEnumerable().Select(a => new AccountDTO() {
                               Email = a.account.Email,
                               FirstName = a.account.FirstName,
                               Id = a.account.Id,
                               IsAdmin = a.account.IsAdmin,
                               IsVerified = a.account.IsVerified,
                               LastName = a.account.LastName,
                               PhoneNumber = a.account.PhoneNumber,
                               Lat = a.accountAppliance.Lat,
                               Lon = a.accountAppliance.Lon,
                               DeviceName = a.accountAppliance.DeviceName,
                               IsVerifiedMobile = a.accountAppliance.IsVerified
                           });

            return resutlt.ToList();
        }

        public List<AccountAppliance> GetAccountApplianceByUUID(string uuId)
        {
            return repoAccountAppliance.FindAll(x => x.IdentifierForVendor == uuId).ToList();
        }

        public Account GetAccountByUUID(string uuId)
        {
            var result = (from accapp in repoAccountAppliance.Table
                          join acc in repoAccount.Table on accapp.AccountId equals acc.Id
                          where accapp.IdentifierForVendor == uuId
                          select new
                          {
                              account = acc,
                          }).AsEnumerable().Select(a => new Account()
                          {
                              Email = a.account.Email,
                              FirstName = a.account.FirstName,
                              Id = a.account.Id,
                              IsAdmin = a.account.IsAdmin,
                              IsVerified = a.account.IsVerified,
                              LastName = a.account.LastName,
                              PhoneNumber = a.account.PhoneNumber
                          });
            return result.FirstOrDefault();
        }

        public List<ApplianceDTO> GetApplianceByAccountId(long id)
        {
            var resutlt = (from accapp in repoAccountAppliance.Table
                           join app in repoAppliance.Table on accapp.ApplianceId equals app.Id
                           where accapp.AccountId == id 
                           //&& accapp.IsQRCodeScaned == true && accapp.IsVerified == true
                           select new
                           {
                               appliance = app,
                               accountAppliance = accapp
                           }).AsEnumerable().Select(a => new ApplianceDTO()
                           {
                               AccountId = a.appliance.AccountId,
                               Cellular = a.appliance.Cellular,
                               City = a.appliance.City,
                               DeviceName = a.appliance.DeviceName,
                               GeoFenceEnabled = a.appliance.GeoFenceEnabled,
                               Id = a.appliance.Id,
                               IsConnected = a.appliance.IsConnected,
                               Lat = a.appliance.Lat,
                               Lon = a.appliance.Lon,
                               OsVersion = a.appliance.OsVersion,
                               Power = a.appliance.Power,
                               SerialNumber = a.appliance.SerialNumber,
                               StateId = a.appliance.StateId,
                               Street1 = a.appliance.Street1,
                               Street2 = a.appliance.Street2,
                               TimerEnabled = a.appliance.TimerEnabled,
                               TriggerMile = a.appliance.TriggerMile,
                               TrustLevel = a.appliance.TrustLevel,
                               Wifi = a.appliance.Wifi,
                               WiFiInternet = a.appliance.WiFiInternet,
                               ZipCode = a.appliance.ZipCode,
                               AirGapVersion = a.accountAppliance.AirGapVersion
                           });

            return resutlt.ToList();
        }

        public AccountAppliance Insert(AccountAppliance accountAppliance)
        {
            return repoAccountAppliance.Insert(accountAppliance);
        }

        public void Remote(long applianceId)
        {
            var accountAppliance = repoAccountAppliance.FindAll(x => x.ApplianceId == applianceId);
            if(accountAppliance != null && accountAppliance.Count() > 0)
            {
                foreach (var item in accountAppliance.ToList())
                {
                    repoAccountAppliance.Delete(item);
                }
            }
        }

        public AccountAppliance Update(AccountAppliance accountAppliance)
        {
            return repoAccountAppliance.Update(accountAppliance);
        }

        public AccountAppliance GetAccountApplianceByAccountIdAndApplianceId(long accountId, long applianceId)
        {
            return repoAccountAppliance.Find(x => x.AccountId == accountId && x.ApplianceId == applianceId);
        }

        public void RemoteUser(long accountId, long applianceId)
        {
            var accountAppliance = repoAccountAppliance.Find(x => x.ApplianceId == applianceId && x.AccountId == accountId);
            repoAccountAppliance.Delete(accountAppliance);
        }

        public List<ApplianceDTO> GetApplianceBySubscriptionId(string subscriptionId)
        {
            var resutlt = (from accapp in repoAccountAppliance.Table
                           join app in repoAppliance.Table on accapp.ApplianceId equals app.Id
                           where accapp.SubscriptionId == subscriptionId
                           //&& accapp.IsQRCodeScaned == true && accapp.IsVerified == true
                           select new
                           {
                               appliance = app,
                               accountAppliance = accapp
                           }).AsEnumerable().Select(a => new ApplianceDTO()
                           {
                               AccountId = a.appliance.AccountId,
                               Cellular = a.appliance.Cellular,
                               City = a.appliance.City,
                               DeviceName = a.appliance.DeviceName,
                               GeoFenceEnabled = a.appliance.GeoFenceEnabled,
                               Id = a.appliance.Id,
                               IsConnected = a.appliance.IsConnected,
                               Lat = a.appliance.Lat,
                               Lon = a.appliance.Lon,
                               OsVersion = a.appliance.OsVersion,
                               Power = a.appliance.Power,
                               SerialNumber = a.appliance.SerialNumber,
                               StateId = a.appliance.StateId,
                               Street1 = a.appliance.Street1,
                               Street2 = a.appliance.Street2,
                               TimerEnabled = a.appliance.TimerEnabled,
                               TriggerMile = a.appliance.TriggerMile,
                               TrustLevel = a.appliance.TrustLevel,
                               Wifi = a.appliance.Wifi,
                               WiFiInternet = a.appliance.WiFiInternet,
                               ZipCode = a.appliance.ZipCode,
                               AirGapVersion = a.accountAppliance.AirGapVersion
                           });

            return resutlt.ToList();
        }

        public List<AccountAppliance> GetAccountApplianceBySubScription(string subscriptionId)
        {
            return repoAccountAppliance.FindAll(x => x.SubscriptionId == subscriptionId).ToList();
        }

        public List<ApplianceDTO> GetAllApplianceOfUUID(string uuId)
        {
            var resutlt = (from accapp in repoAccountAppliance.Table
                           join app in repoAppliance.Table on accapp.ApplianceId equals app.Id
                           where accapp.IdentifierForVendor == uuId
                           select new
                           {
                               appliance = app,
                           }).AsEnumerable().Select(a => new ApplianceDTO()
                           {
                               AccountId = a.appliance.AccountId,
                               Cellular = a.appliance.Cellular,
                               City = a.appliance.City,
                               DeviceName = a.appliance.DeviceName,
                               GeoFenceEnabled = a.appliance.GeoFenceEnabled,
                               Id = a.appliance.Id,
                               IsConnected = a.appliance.IsConnected,
                               Lat = a.appliance.Lat,
                               Lon = a.appliance.Lon,
                               OsVersion = a.appliance.OsVersion,
                               Power = a.appliance.Power,
                               SerialNumber = a.appliance.SerialNumber,
                               StateId = a.appliance.StateId,
                               Street1 = a.appliance.Street1,
                               Street2 = a.appliance.Street2,
                               TimerEnabled = a.appliance.TimerEnabled,
                               TriggerMile = a.appliance.TriggerMile,
                               TrustLevel = a.appliance.TrustLevel,
                               Wifi = a.appliance.Wifi,
                               WiFiInternet = a.appliance.WiFiInternet,
                               ZipCode = a.appliance.ZipCode
                           }).ToList();
            return resutlt;
        }
    }
}
