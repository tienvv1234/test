using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Entity;
using Airgap.Entity.Entities;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using Airgap.Data.ApiEntities;
using Airgap.Service;
using Airgap.Data.DTOEntities;
using Airgap.Telit;

namespace Airgap.WebApi.Controllers
{
    public class AirController : BaseController
    {
        private IAccountApplianceService _accountApplianceService;
        private ITimerScheduleService _timerScheduleService;
        private IStateService _stateService;
        private INotificationPreferenceService _notificationPreferenceService;
        private IApplianceService _applianceService;
        private IEventService _eventService;
        private INotificationService _notificationService;
        private IAccountService _accountService;

        private List<ApplianceDTO> listAccountAppliance = null;
        private List<TimerScheduleDTO> listTimerSchedule = null;
        private List<AccountDTO> ListAccount = null;
        private List<State> listStates = null;
        private List<NotificationPreference> listNotificationPreference = null;

        public AirController(IAccountService accountService, INotificationService notificationService, IEventService eventService, INotificationPreferenceService notificationPreferenceService, IApplianceService applianceService, IStateService stateService, ITimerScheduleService timerScheduleService, IAccountApplianceService accountApplianceService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._applianceService = applianceService;
            this._accountApplianceService = accountApplianceService;
            this._timerScheduleService = timerScheduleService;
            this._stateService = stateService;
            this._notificationPreferenceService = notificationPreferenceService;
            this._eventService = eventService;
            this._notificationService = notificationService;
            this._accountService = accountService;
        }

        private void InitialData(string accountid, string applianceid)
        {
            listAccountAppliance = _accountApplianceService.GetApplianceByAccountId(Convert.ToInt64(accountid));
            listTimerSchedule = _timerScheduleService.GetTimerScheduleDTOByApplianceId(Convert.ToInt64(applianceid));
            ListAccount = _accountApplianceService.GetAccountByApplianceId(Convert.ToInt64(applianceid), true);
            listStates = _stateService.GetAllState();
            listNotificationPreference = _notificationPreferenceService.GetNotificationPreferenceByApplianceId(Convert.ToInt64(applianceid));
        }

        public ResponseData<AirGapSetting> Index(string accountid, string applianceid)
        {
            try
            {
                InitialData(accountid, applianceid);
                foreach (var account in ListAccount)
                {
                    if (account.Id == Convert.ToInt64(accountid))
                        continue;

                    foreach (var noti in listNotificationPreference)
                    {
                        if (noti.AccountId == account.Id)
                        {
                            switch (noti.EventTypeId)
                            {
                                case Constant.EventType.NetWorkStatusChange:
                                    account.NetWorkStatusChange = true;
                                    break;
                                case Constant.EventType.HomePowerLoss:
                                    account.HomePowerLoss = true;
                                    break;
                                case Constant.EventType.ISPOutage:
                                    account.ISPOutage = true;
                                    break;
                            }
                        }
                    }
                }

                ListAccount = ListAccount.Where(x => x.Id != Convert.ToInt64(accountid)).ToList();

                var airGapSetting = new AirGapSetting()
                {
                    ListApplianceDTO = listAccountAppliance,
                    ListAccountDTO = ListAccount,
                    TimerScheduleDTO = listTimerSchedule,
                    States = listStates,
                };

                var response = new ResponseData<AirGapSetting>();
                response.Data = airGapSetting;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AirGapSetting>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<AirGapSetting> deleteSerialNumber(string applianceId, string accountId)
        {
            try
            {
                _accountApplianceService.Remote(Convert.ToInt64(applianceId));

                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceId));
                appliance.AccountId = null;
                appliance.Status = false;
                _applianceService.Update(appliance);
                var listAccountAppliance = _accountApplianceService.GetApplianceByAccountId(Convert.ToInt64(accountId));
                var airGapSetting = new AirGapSetting()
                {
                    ListApplianceDTO = listAccountAppliance
                };

                _notificationPreferenceService.RemoveNotificationPreferenceByApplianceId(Convert.ToInt64(applianceId));
                _notificationService.RemoveNotificationByApplianceId(Convert.ToInt64(applianceId));
                _eventService.RemoveEventByApplianceId(Convert.ToInt64(applianceId));

                var response = new ResponseData<AirGapSetting>();
                response.Data = airGapSetting;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AirGapSetting>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public ResponseData<AirGapSetting> UpdateNotificationPreference(string value, string applianceId)
        {
            try
            {
                _notificationPreferenceService.RemoveNotificationPreferenceByApplianceId(Convert.ToInt64(applianceId));
                if(!string.IsNullOrEmpty(value))
                {
                    if(value.IndexOf(',') != -1)
                    {
                        var arr = value.Split(',');
                        foreach (var item in arr)
                        {
                            var temp = item.Split('-');
                            string accountId = temp[0];
                            string timerTypeId = temp[1];
                            NotificationPreference notificationPreference = new NotificationPreference()
                            {
                                AccountId = Convert.ToInt64(accountId),
                                ApplianceId = Convert.ToInt64(applianceId),
                                EventTypeId = Convert.ToInt64(timerTypeId),
                            };
                            _notificationPreferenceService.Insert(notificationPreference);
                        }
                    }
                    else
                    {
                        var arr = value.Split('-');
                        string accountId = arr[0];
                        string timerTypeId = arr[1];
                        NotificationPreference notificationPreference = new NotificationPreference()
                        {
                            AccountId = Convert.ToInt64(accountId),
                            ApplianceId = Convert.ToInt64(applianceId),
                            EventTypeId = Convert.ToInt64(timerTypeId),
                        };
                        _notificationPreferenceService.Insert(notificationPreference);
                    }
                }

                var response = new ResponseData<AirGapSetting>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AirGapSetting>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        [HttpPost]
        public async Task<ResponseData<AirGapSetting>> CompareAddressFromTelit([FromBody]ApplianceDTO appliance)
        {
            try
            {
                InitialData(appliance.AccountId.ToString(), appliance.Id.ToString());

                var response = new ResponseData<AirGapSetting>();
                var app = _applianceService.GetApplianceById(appliance.Id);
                var things = await TelitApi.ThingList();
                bool isMatching = false;
                Telit.ThingList.Result thing = new Telit.ThingList.Result();
                if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                {
                    foreach (var item in things.things.@params.result)
                    {
                        if (item.key == app.SerialNumber)
                        {
                            if (item.loc != null && item.loc.addr != null)
                            {
                                isMatching = await calculateAddressAsync(appliance.Street1.Trim(), appliance.City.Trim(), _stateService.GetStateById(appliance.StateId.Value).Name, appliance.ZipCode.Trim(), item.loc.lat, item.loc.lng);
                                thing = item;
                                break;
                            }
                        }
                    }
                }


                if (isMatching)
                {
                    app.Street1 = appliance.Street1;
                    app.Street2 = appliance.Street2;
                    app.StateId = appliance.StateId;
                    app.City = appliance.City;
                    app.ZipCode = appliance.ZipCode;
                    var locationElement = await GetLatLngByAddressAsync(appliance.Street1.Trim(), appliance.City.Trim(), _stateService.GetStateById(appliance.StateId.Value).Name, appliance.ZipCode.Trim());
                    app.City = appliance.City;
                    app.Lat = Convert.ToDouble(locationElement.Element("lat").Value);
                    app.Lon = Convert.ToDouble(locationElement.Element("lng").Value);
                    _applianceService.Update(app);
                }

                foreach (var account in ListAccount)
                {
                    if (account.Id == appliance.AccountId)
                        continue;

                    foreach (var noti in listNotificationPreference)
                    {
                        if (noti.AccountId == account.Id)
                        {
                            switch (noti.EventTypeId)
                            {
                                case Constant.EventType.NetWorkStatusChange:
                                    account.NetWorkStatusChange = true;
                                    break;
                                case Constant.EventType.HomePowerLoss:
                                    account.HomePowerLoss = true;
                                    break;
                                case Constant.EventType.ISPOutage:
                                    account.ISPOutage = true;
                                    break;
                            }
                        }
                    }
                }

                ListAccount = ListAccount.Where(x => x.Id != appliance.AccountId).ToList();

                var airGapSetting = new AirGapSetting()
                {
                    ListApplianceDTO = listAccountAppliance,
                    ListAccountDTO = ListAccount,
                    TimerScheduleDTO = listTimerSchedule,
                    IsMatching = isMatching
                };

                response.Data = airGapSetting;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AirGapSetting>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public async Task<ResponseData<AirGapSetting>> UseAddressOfTelit([FromBody]ApplianceDTO appliance)
        {
            try
            {
                InitialData(appliance.AccountId.ToString(), appliance.Id.ToString());

                var response = new ResponseData<AirGapSetting>();
                var app = _applianceService.GetApplianceById(appliance.Id);
                var things = await TelitApi.ThingList();
                Telit.ThingList.Result thing = new Telit.ThingList.Result();
                if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                {
                    foreach (var item in things.things.@params.result)
                    {
                        if (item.key == app.SerialNumber)
                        {
                            app.Street1 = item.loc != null && item.loc.addr != null ? item.loc.addr.streetNumber + " " + item.loc.addr.street : string.Empty;
                            app.StateId = item.loc != null && item.loc.addr != null && _stateService.GetStateByNameOrCode(item.loc.addr.state) != null ? (long?)_stateService.GetStateByNameOrCode(item.loc.addr.state).Id : null;
                            app.Street2 = string.Empty;
                            app.City = item.loc != null && item.loc.addr != null ? item.loc.addr.city : string.Empty;
                            app.ZipCode = item.loc != null && item.loc.addr != null ? item.loc.addr.zipCode : null;
                            app.Lat = item.loc != null ? item.loc.lat : 0;
                            app.Lon = item.loc != null ? item.loc.lng : 0;
                            _applianceService.Update(app);
                            break;
                        }
                    }
                }

                foreach (var account in ListAccount)
                {
                    if (account.Id == appliance.AccountId)
                        continue;

                    foreach (var noti in listNotificationPreference)
                    {
                        if (noti.AccountId == account.Id)
                        {
                            switch (noti.EventTypeId)
                            {
                                case Constant.EventType.NetWorkStatusChange:
                                    account.NetWorkStatusChange = true;
                                    break;
                                case Constant.EventType.HomePowerLoss:
                                    account.HomePowerLoss = true;
                                    break;
                                case Constant.EventType.ISPOutage:
                                    account.ISPOutage = true;
                                    break;
                            }
                        }
                    }
                }

                ListAccount = ListAccount.Where(x => x.Id != appliance.AccountId).ToList();

                var airGapSetting = new AirGapSetting()
                {
                    ListApplianceDTO = listAccountAppliance,
                    ListAccountDTO = ListAccount,
                    TimerScheduleDTO = listTimerSchedule
                };

                response.Data = airGapSetting;

                response.Status = ResponseStatus.Success.ToString();
                response.Message = ResponseMessage.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AirGapSetting>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public async Task<ResponseData<SerialNumberExport>> GetAllThingFromTelit()
        {
            try
            {
                var things = await TelitApi.ThingList();
                Telit.ThingList.Result thing = new Telit.ThingList.Result();
                List<SerialNumberExport> listSerialNumber = new List<SerialNumberExport>();
                var response = new ResponseData<SerialNumberExport>();
                if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                {
                    foreach (var thingItem in things.things.@params.result)
                    {
                        var serialNumber = new SerialNumberExport();
                        serialNumber.SerialNumber = thingItem.key;

                        var appliance = _applianceService.GetApplianceBySerialNumber(thingItem.key);
                        if(appliance != null)
                        {
                            var account = appliance.AccountId != null && appliance.AccountId.HasValue ? _accountService.GetAccountById(appliance.AccountId.Value) : null;
                            if (account != null)
                            {
                                serialNumber.AccountOwner = account.Password != null ? account.Email : null;
                                serialNumber.FaceBook = account.Password == null ? account.Email : null;
                            }
                        }
                        listSerialNumber.Add(serialNumber);
                    }
                }

                response.DataList = listSerialNumber;
                response.Status = ResponseStatus.Success.ToString();
                response.Message = ResponseMessage.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<SerialNumberExport>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

    }
}