using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Entity;
using Airgap.Entity.Entities;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using Airgap.Service;
using Airgap.Data.DTOEntities;
using Airgap.WebApi.Helper;
using Airgap.Data.ApiEntities;

namespace Airgap.WebApi.Controllers
{
    public class UserController : BaseController
    {
        private IAccountApplianceService _accountApplianceService;
        private IApplianceService _applianceService;
        private IAccountService _accountService;
        private IEventService _eventService;
        private INotificationService _notificationService;
        public UserController(IApplianceService applianceService, INotificationService notificationService, IEventService eventService, IOptions<AppSetting> appSettings, IAccountApplianceService accountApplianceService, IAccountService accountService) : base(appSettings.Value)
        {
            this._accountApplianceService = accountApplianceService;
            this._accountService = accountService;
            this._eventService = eventService;
            this._notificationService = notificationService;
            this._applianceService = applianceService;
        }

        public ResponseData<User> Index(string applianceId, string accountId)
        {
            try
            {
                var lAppliance = _accountApplianceService.GetApplianceByAccountId(Convert.ToInt64(accountId));
                var lAccount = _accountApplianceService.GetAccountByApplianceId(Convert.ToInt64(applianceId), null);
                var response = new ResponseData<User>();
                var user = new User() {
                    lAccountDTO = lAccount,
                    lAppliance = lAppliance
                };
                response.Data = user;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<User>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<AccountDTO> UpdateUser(string applianceId, string currenctUserId, string accountId, string firstName, string lastName, string isVerified)
        {
            try
            {
                var account = _accountService.GetAccountById(Convert.ToInt64(accountId));
                account.FirstName = firstName;
                account.LastName = lastName;
                _accountService.Update(account);
                bool temp = Convert.ToBoolean(isVerified);
                var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId (Convert.ToInt64(accountId), Convert.ToInt64(applianceId));
                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceId));
                if (temp != accountAppliance.IsVerified)
                {
                    accountAppliance.IsVerified = temp;
                    var _event = new Event()
                    {
                        AccountId = Convert.ToInt64(currenctUserId),
                        ApplianceId = Convert.ToInt64(applianceId),
                        EventTypeId = Constant.EventType.AccountVerifyForAppliance,
                        Timestamp = DateTime.UtcNow,
                        Message = appliance.DeviceName + "(" + appliance.SerialNumber.Substring(appliance.SerialNumber.Length - 4) + ")" + ": " + (temp ? "Add " + account.PhoneNumber : "Remove " + account.PhoneNumber)
                    };

                    _eventService.Insert(_event);

                    if (accountAppliance.PhoneType == Configuration.Android)
                    {
                        var pushGcmotification = new AndroidGcmPushNotification(_appSettings);
                        pushGcmotification.InitPushNotification(accountAppliance.DeviceToken, _event.Message);
                    }
                    else
                    {
                        var applePush = new ApplePushNotificationService(_appSettings);
                        applePush.PushMessage(_event.Message, accountAppliance.DeviceToken, 0, null).Wait();
                    }

                    var notification = new Notification()
                    {
                        AccountId = accountAppliance.AccountId,
                        ApplianceId = accountAppliance.ApplianceId,
                        EventId = _event.Id,
                        Timestamp = DateTime.UtcNow
                    };

                    _notificationService.Insert(notification);
                }

                _accountApplianceService.Update(accountAppliance);
                var response = new ResponseData<AccountDTO>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AccountDTO>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<AccountDTO> DeleteUser(string accountId, string applianceId)
        {
            try
            {
                _accountApplianceService.RemoteUser(Convert.ToInt64(accountId), Convert.ToInt64(applianceId));

                var response = new ResponseData<AccountDTO>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AccountDTO>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }
    }
}