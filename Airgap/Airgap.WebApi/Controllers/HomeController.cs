using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Entity;
using Airgap.Entity.Entities;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using Airgap.Telit;
using Airgap.Service;
using Airgap.Data.ApiEntities;
using Airgap.Data.DTOEntities;
using Airgap.WebApi.Helper;
using System.Text;
using Stripe;

namespace Airgap.WebApi.Controllers
{
    public class HomeController : BaseController
    {
        private IAccountApplianceService _accountApplianceService;
        private IApplianceService _applianceService;
        private IEventService _eventService;
        private ITimerScheduleService _timerScheduleService;
        private IAccountService _accountService;
        private IStripeService _stripeService;

        public HomeController(IStripeService stripeService, IAccountService accountService, ITimerScheduleService timerScheduleService, IApplianceService applianceService, IEventService eventService, IAccountApplianceService accountApplianceService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._accountApplianceService = accountApplianceService;
            this._eventService = eventService;
            this._applianceService = applianceService;
            this._timerScheduleService = timerScheduleService;
            this._accountService = accountService;
            this._stripeService = stripeService;
        }


        public async Task<ResponseData<Dashboard>> Index(string accountid, string applianceid)
        {
            try
            {
                var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountId(Convert.ToInt64(accountid));
                var _event = _eventService.GetEventByApplianceId(Convert.ToInt64(applianceid));
                var lAccounts = _accountApplianceService.GetAccountByApplianceId(Convert.ToInt64(applianceid), true);
                List<AccountApplianceDTO> accountApplianceDTO = new List<AccountApplianceDTO>();
                
                if (accountAppliance != null && accountAppliance.Count > 0)
                {
                    foreach (var item in accountAppliance)
                    {
                        accountApplianceDTO.Add(new AccountApplianceDTO(item));
                    }
                }

                lAccounts = lAccounts.Where(x => x.Id != Convert.ToInt64(accountid)).ToList();

                if (!TelitApi.IsConnected) TelitApi.CheckIOTConnection().Wait();

                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceid));
                var thingFind = await TelitApi.ThingFind(appliance.SerialNumber);



                Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                string applianceEnvironment = string.Empty;

                ApplianceDTO applianceDTO = new ApplianceDTO();

                if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                {
                    _params = thingFind.Things.@params;
                }

                if (_params != null && _params.alarms != null && _params.alarms.env != null && _params.alarms.env.state >= 0 && _params.alarms.env.state < 16)
                {
                    applianceEnvironment = Convert.ToString(_params.alarms.env.state, 2).PadLeft(4, '0');
                    var array = !string.IsNullOrEmpty(applianceEnvironment) ? applianceEnvironment.ToArray() : new char[] { };

                    applianceDTO.Cellular = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[2].ToString())) : false;
                    applianceDTO.Power = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false;
                    applianceDTO.Wifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;
                    applianceDTO.WiFiInternet = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false;

                }

                applianceDTO.TimerEnabled = _params != null && _params.alarms != null && _params.alarms.timerState != null ? (Convert.ToInt64(_params.alarms.timerState.state) != 0 ? true : false) : false;
                applianceDTO.DeviceName = _params != null && _params.attrs != null && _params.attrs.name != null ? _params.attrs.name.value : appliance.DeviceName;
                applianceDTO.SerialNumber = appliance.SerialNumber;
                applianceDTO.TriggerMile = appliance.TriggerMile;
                applianceDTO.Id = appliance.Id;
                applianceDTO.IsOn = _params != null && _params.alarms != null && _params.alarms.on != null ? CheckApplianceIsOn(Convert.ToInt16(_params.alarms.on.state)) : false;
                applianceDTO.IsConnected = _params.connected;
                applianceDTO.Lat = appliance.Lat;
                applianceDTO.Lon = appliance.Lon;
                applianceDTO.GeoFenceEnabled = appliance.GeoFenceEnabled;
                applianceDTO.Status = appliance.Status;

                if (_params != null && _params.alarms != null)
                {
                    applianceDTO.TrustLevel = _params.alarms.trust != null ? (int?)_params.alarms.trust.state : 3;
                }

                var plans = _stripeService.GetPlans();

                var subscriptionId = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(Convert.ToInt64(accountid), Convert.ToInt64(applianceid));
                DateTime? expireDate = null;
                bool cancelAtEnd = false;
                if (subscriptionId != null && !string.IsNullOrEmpty(subscriptionId.SubscriptionId))
                {
                    var stripe = _stripeService.RetrieveSubscription(subscriptionId.SubscriptionId);
                    // current period end
                    expireDate = stripe.CurrentPeriodEnd;
                    cancelAtEnd = stripe.CancelAtPeriodEnd;
                }

                var dashboard = new Dashboard()
                {
                    AccountAppliance = accountApplianceDTO,
                    Events = _event,
                    ListAccountsDTO = lAccounts,
                    IsIOTConnected = TelitApi.IsConnected,
                    Plans = plans,
                    AppDTO = applianceDTO,
                    ExpireDate = expireDate,
                    CancelAtEnd = cancelAtEnd,
                    IsApplianceConnected = applianceDTO.IsConnected.Value
                };

                var response = new ResponseData<Dashboard>();

                response.Data = dashboard;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        private bool CheckApplianceIsOn(int state)
        {
            if(state == (Int32)AlarmStatus.OffAppliance || state == (Int32)AlarmStatus.OffGeofence || state == (Int32)AlarmStatus.OffMobile || state == (Int32)AlarmStatus.OffPortal || state == (Int32)AlarmStatus.OffReserved || state == (Int32)AlarmStatus.OffScheduleTimer || state == (Int32)AlarmStatus.OffUnknow)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<ResponseData<Dashboard>> UpdateAppliance(string applianceid, string accountId, string power)
        {
            try
            {
                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceid));
                var account = _accountService.GetAccountById(appliance.AccountId.Value);

                if (appliance.GeoFenceEnabled != null && appliance.GeoFenceEnabled.HasValue && !appliance.GeoFenceEnabled.Value && appliance.TimerEnabled != null && appliance.TimerEnabled.HasValue && !appliance.TimerEnabled.Value)
                {
                    appliance.GeoFenceEnabled = true;
                    appliance.TimerEnabled = true;
                    _applianceService.Update(appliance);
                    if (appliance.IsOn.Value)
                    {
                        var updateTimerState = await TelitApi.UpdateTimerState(appliance.SerialNumber, Configuration.TimerEnableAirgapOn, false);
                    }
                }

                string message = "Update from WebPortal by " + account.Email;
                var isSuccess = await TelitApi.UpdateAlarmState(appliance.SerialNumber, power == Configuration.TurnOff.ToLower() ? AlarmStatus.OffPortal : AlarmStatus.OnPortal, message);

                var _event = new Event()
                {
                    ApplianceId = Convert.ToInt64(applianceid),
                    AccountId = Convert.ToInt64(accountId),
                    EventTypeId = Constant.EventType.ManualConnectedStatusChange,
                    EventDetail = Constant.ResponseMessage.ManualConnectedStatusChange,
                    Timestamp = DateTime.UtcNow,
                    Message = Constant.ResponseMessage.Wifi + " " + power.ToUpper()
                };

                _eventService.Insert(_event);


                var response = new ResponseData<Dashboard>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public async Task<ResponseData<Dashboard>> SetAirGapOn(string applianceid, string accountId, string airGapAlwaysStatus)
        {
            try
            {
                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceid));
                var account = _accountService.GetAccountById(appliance.AccountId.Value);
                var thingFind = await TelitApi.ThingFind(appliance.SerialNumber);

                Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                string applianceEnvironment = string.Empty;


                if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                {
                    _params = thingFind.Things.@params;

                }
                bool isOn = _params != null && _params.alarms != null && _params.alarms.on != null ? CheckApplianceIsOn(Convert.ToInt16(_params.alarms.on.state)) : false;

                if (airGapAlwaysStatus.ToLower() == Configuration.TurnOff.ToLower())
                {
                    appliance.TimerEnabled = true;
                    appliance.GeoFenceEnabled = true;
                    var isSuccess = await TelitApi.UpdateTimerState(appliance.SerialNumber, Configuration.TimerEnable, appliance.IsOn.Value);
                }
                else
                {
                    if (isOn == false)
                    {
                        string message = "Update from WebPortal by " + account.Email;
                        var isSuccess = await TelitApi.UpdateAlarmState(appliance.SerialNumber, airGapAlwaysStatus == Configuration.TurnOff.ToLower() ? AlarmStatus.OffPortal : AlarmStatus.OnPortal, message);
                        var updateTimerState = await TelitApi.UpdateTimerState(appliance.SerialNumber, Configuration.TimerDisable, appliance.IsOn.Value);
                    }
                    else
                    {
                        var updateTimerState = await TelitApi.UpdateTimerState(appliance.SerialNumber, Configuration.TimerDisable, appliance.IsOn.Value);
                    }

                    appliance.TimerEnabled = false;
                    appliance.GeoFenceEnabled = false;
                }

                _applianceService.Update(appliance);

                var _event = new Event()
                {
                    ApplianceId = Convert.ToInt64(applianceid),
                    AccountId = Convert.ToInt64(accountId),
                    EventTypeId = Constant.EventType.AlwaysOnStatusChange,
                    EventDetail = Constant.ResponseMessage.AlwaysOnStatusChange,
                    Timestamp = DateTime.UtcNow,
                    Message = Constant.ResponseMessage.AlwaysOnStatusChange + " " + airGapAlwaysStatus.ToUpper()
                };

                _eventService.Insert(_event);

                var response = new ResponseData<Dashboard>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public ResponseData<Dashboard> Filler(string applianceid, string dateFrom, string dateTo)
        {
            try
            {
                DateTime? from = null;
                DateTime? to = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    from = DateTime.Parse(dateFrom.Replace('-', '/'));
                }

                if (!string.IsNullOrEmpty(dateTo))
                {
                    to = DateTime.Parse(dateTo.Replace('-', '/'));
                }

                var lEvent = _eventService.GetEventByDate(Convert.ToInt64(applianceid), from, to);
                var acc = _applianceService.GetApplianceById(Convert.ToInt64(applianceid));
                List<ApplianceDTO> lAppliance = new List<ApplianceDTO>();
                lAppliance.Add(new ApplianceDTO(acc));
                var dashboard = new Dashboard()
                {
                    Events = lEvent,
                    ApplianceDTO = lAppliance
                };
                var response = new ResponseData<Dashboard>();
                response.Data = dashboard;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<Dashboard> AddUUIDBySerialNumber([FromBody]RequestScanQRCode request)
        {
            try
            {
                bool isNew = true;
                var response = new ResponseData<Dashboard>();
                var appliance = _applianceService.GetApplianceBySerialNumber(request.serialnumber);
                if (appliance != null)
                {
                    var lAccountAppliance = _accountApplianceService.GetAccountApplianceByUUID(request.uuid);
                    if (lAccountAppliance != null && lAccountAppliance.Count > 0)
                    {
                        foreach (var item in lAccountAppliance)
                        {
                            if (item.ApplianceId == appliance.Id)
                            {
                                isNew = false;
                            }
                        }

                        if (isNew)
                        {
                            var accAppliance = new AccountAppliance()
                            {
                                IdentifierForVendor = request.uuid,
                                IsQRCodeScaned = true,
                                DeviceName = request.devicename,
                                Lat = Convert.ToDouble(request.latitude),
                                Lon = Convert.ToDouble(request.longitude),
                                ApplianceId = appliance.Id,
                                AccountId = lAccountAppliance.FirstOrDefault().AccountId,
                                PhoneType = request.phoneType,
                                DeviceToken = request.devicetoken
                            };
                            _accountApplianceService.Insert(accAppliance);

                            var account = _accountService.GetAccountById(accAppliance.AccountId.Value);

                            //Record event QR code
                            var _event = new Event()
                            {
                                AccountId = accAppliance.AccountId,
                                ApplianceId = accAppliance.ApplianceId,
                                EventTypeId = Constant.EventType.ApplianceQRCodeScanned,
                                Timestamp = DateTime.UtcNow,
                                Message = account.PhoneNumber + " Pending"
                            };
                            _eventService.Insert(_event);
                        }
                        else
                        {
                            response.Status = ResponseStatus.Existed.ToString();
                            response.Message = ResponseMessage.SerialNumberIsExist;
                            return response;
                        }
                    }
                    else
                    {
                        
                        var acc = _accountService.GetAccountByPhoneNumber(request.phonenumber);

                        var accAppliance = new AccountAppliance()
                        {
                            IdentifierForVendor = request.uuid,
                            IsQRCodeScaned = true,
                            DeviceName = request.devicename,
                            Lat = Convert.ToDouble(request.latitude),
                            Lon = Convert.ToDouble(request.longitude),
                            ApplianceId = appliance.Id,
                            AccountId = acc.Id,
                            PhoneType = request.phoneType,
                            DeviceToken = request.devicetoken
                        };
                        _accountApplianceService.Insert(accAppliance);

                        //Record event QR code
                        var _event = new Event()
                        {
                            AccountId = accAppliance.AccountId,
                            ApplianceId = accAppliance.ApplianceId,
                            EventTypeId = Constant.EventType.ApplianceQRCodeScanned,
                            Timestamp = DateTime.UtcNow,
                            Message = request.phonenumber + " Pending"
                        };
                        _eventService.Insert(_event);
                    }

                    response.Message = ResponseMessage.Success;
                }
                else
                {
                    response.Message = ResponseMessage.SerialNumberInCorrect;
                }
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public async Task<ResponseData<Dashboard>> DashBoardForMobile(string uuid)
        {
            try
            {
                var response = new ResponseData<Dashboard>();

                var lAppliance = _accountApplianceService.GetApplianceByUUID(uuid);
                if (!TelitApi.IsConnected) await TelitApi.CheckIOTConnection();
                if (lAppliance != null && lAppliance.Count() > 0)
                {
                    foreach (var item in lAppliance)
                    {
                        var thingFind = await TelitApi.ThingFind(item.SerialNumber);
                        Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                        string applianceEnvironment = string.Empty;
                        if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                        {
                            _params = thingFind.Things.@params;
                        }

                        if (_params != null && _params.alarms != null && _params.alarms.env != null && _params.alarms.env.state >= 0 && _params.alarms.env.state < 16)
                        {
                            applianceEnvironment = Convert.ToString(_params.alarms.env.state, 2).PadLeft(4, '0');
                            var array = !string.IsNullOrEmpty(applianceEnvironment) ? applianceEnvironment.ToArray() : new char[] { };
                            item.IsOn = _params != null && _params.alarms != null && _params.alarms.on != null ? CheckApplianceIsOn(Convert.ToInt16(_params.alarms.on.state)) : false;
                            item.IsConnected = _params.connected;
                            item.Cellular = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[2].ToString())) : false;
                            item.Power = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false;
                            item.Wifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;
                            item.WiFiInternet = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false;
                            item.TrustLevel = _params.alarms.trust != null ? (int?)_params.alarms.trust.state : null;
                        }
                    }
                }

                var dashboard = new Dashboard()
                {
                    ApplianceDTO = lAppliance,
                    IsIOTConnected = TelitApi.IsConnected
                };
                response.Data = dashboard;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<Dashboard> IsPhoneNumberExist(string uuid)
        {
            try
            {
                var response = new ResponseData<Dashboard>();

                var account = _accountApplianceService.GetAccountByUUID(uuid);
                if (account != null && !string.IsNullOrEmpty(account.PhoneNumber))
                {
                    response.Status = ResponseStatus.Existed.ToString();
                    response.Message = ResponseMessage.PhoneNumberExist;
                }
                else
                {
                    response.Status = ResponseStatus.Success.ToString();
                    response.Message = ResponseMessage.Success;
                }


                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<Dashboard> GetFileLogByMonthForMobile(string applianceid)
        {
            try
            {
                DateTime from = DateTime.UtcNow.AddDays(-30);
                DateTime to = DateTime.UtcNow;

                var lEvent = _eventService.GetEventByDate(Convert.ToInt64(applianceid), from, to);
                var dashboard = new Dashboard()
                {
                    Events = lEvent,
                };
                var response = new ResponseData<Dashboard>();
                response.Data = dashboard;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public async Task<ResponseData<Dashboard>> UpdateApplianceForMobile(string applianceid, string uuId, string power)
        {
            try
            {
                var appliance = _applianceService.GetApplianceById(Convert.ToInt64(applianceid));
                var account = _accountApplianceService.GetAccountByUUID(uuId);

                string message = "Update from Mobile App by " + account.FirstName;
                var isSuccess = await TelitApi.UpdateAlarmState(appliance.SerialNumber, power == Configuration.TurnOff.ToLower() ? AlarmStatus.OffMobile : AlarmStatus.OnMobile, message);


                var _event = new Event()
                {
                    ApplianceId = Convert.ToInt64(applianceid),
                    AccountId = Convert.ToInt64(account.Id),
                    EventTypeId = Constant.EventType.ManualConnectedStatusChange,
                    EventDetail = Constant.ResponseMessage.ManualConnectedStatusChange,
                    Timestamp = DateTime.UtcNow,
                    Message = Constant.ResponseMessage.Wifi + " " + power.ToUpper()
                };

                _eventService.Insert(_event);


                var response = new ResponseData<Dashboard>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public ResponseData<StripePlan> Purchase([FromBody] PurchaseInformation info)
        {
            try
            {
                var response = new ResponseData<StripePlan>();
                var account = _accountService.GetAccountById(info.CurrenUserId);
                if (string.IsNullOrEmpty(account.CustomerIdStripe))
                {
                    var stripeCustomer = _stripeService.CreateUser(info, account.Email);
                    account.CustomerIdStripe = stripeCustomer.Id;
                    _accountService.Update(account);
                    var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(info.CurrenUserId, info.CurrenApplianceId);
                    accountAppliance.SubscriptionId = stripeCustomer.Subscriptions.Data.FirstOrDefault().Id;
                    _accountApplianceService.Update(accountAppliance);
                }
                else
                {
                    var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(info.CurrenUserId, info.CurrenApplianceId);

                    if (!string.IsNullOrEmpty(accountAppliance.SubscriptionId))
                    {
                        var subscriptionService = _stripeService.RetrieveSubscription(accountAppliance.SubscriptionId);
                        if (subscriptionService != null && subscriptionService.Status == StripeSubscriptionStatuses.Active || subscriptionService.Status == StripeSubscriptionStatuses.Trialing)
                        {
                            var subscription = _stripeService.UpdateSubscription(info, account.CustomerIdStripe, accountAppliance.SubscriptionId);
                        }
                        else
                        {
                            var subscription = _stripeService.CreateSubscription(info, account.CustomerIdStripe);
                            accountAppliance.SubscriptionId = subscription.Id;
                            _accountApplianceService.Update(accountAppliance);
                        }

                    }
                    else
                    {
                        var subscription = _stripeService.CreateSubscription(info, account.CustomerIdStripe);
                        accountAppliance.SubscriptionId = subscription.Id;
                        _accountApplianceService.Update(accountAppliance);
                    }

                }

                var plan = _stripeService.GetPlansByPlanId(info.PlanId);
                if (!string.IsNullOrEmpty(info.Coupon))
                {
                    var coupOn = _stripeService.RetrieveCoupon(info.Coupon);
                    if(coupOn.PercentOff != null && coupOn.PercentOff.HasValue)
                    {
                        plan.Amount = plan.Amount - (plan.Amount * coupOn.PercentOff.Value / 100);
                    }
                    else
                    {
                        plan.Amount = plan.Amount - coupOn.AmountOff.Value;
                    }

                }
                response.Data = plan;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<StripePlan>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public async Task<ResponseData<SerialNumberDTO>> CheckSerialNumber([FromBody] SerialNumberDTO dto)
        {
            try
            {
                List<string> serialNumberExist = new List<string>();
                List<string> serialNumberNotExist = new List<string>();
                string[] lines = dto.SerialNumberInput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var item in lines)
                {
                    var serialnumber = _applianceService.GetApplianceBySerialNumber(item);

                    if (serialnumber != null && serialnumber.AccountId > 0)
                    {
                        var things = await TelitApi.ThingList();
                        Telit.ThingList.Result thing = new Telit.ThingList.Result();
                        if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                        {
                            foreach (var thingItem in things.things.@params.result)
                            {
                                if (thingItem.key == serialnumber.SerialNumber)
                                {
                                    serialNumberExist.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        var things = await TelitApi.ThingList();

                        Telit.ThingList.Result thing = new Telit.ThingList.Result();
                        if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                        {
                            foreach (var thingItem in things.things.@params.result)
                            {
                                if (thingItem.key == item)
                                {
                                    serialNumberNotExist.Add(item);
                                }
                            }
                        }
                    }
                }

                dto.SerialNumberExist = serialNumberExist.ToArray<string>();
                dto.SerialNumberNotExist = serialNumberNotExist.ToArray<string>();

                var response = new ResponseData<SerialNumberDTO>();
                response.Data = dto;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<SerialNumberDTO>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public ResponseData<Dashboard> CheckCoupon(string coupon)
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                List<StripePlan> lPlan = null;
                var coupOn = _stripeService.RetrieveCoupon(coupon);
                if (coupOn != null)
                {
                    var plans = _stripeService.GetPlans();
                    if (plans != null && plans.Count > 0)
                    {
                        foreach (var item in plans)
                        {
                            if (coupOn.PercentOff != null && coupOn.PercentOff.HasValue)
                            {
                                item.Amount = item.Amount - (item.Amount * coupOn.PercentOff.Value / 100);
                            }

                            if(coupOn.AmountOff != null && coupOn.AmountOff.HasValue)
                            {
                                item.Amount = item.Amount - coupOn.AmountOff.Value;
                            }
                        }
                        lPlan = plans;
                        response.Message = ResponseMessage.Success;
                    }
                }
                else
                {
                    response.Message = ResponseMessage.CoupOnInvalid;
                }

                Dashboard dashboard = new Dashboard()
                {
                    Plans = lPlan,
                    PercentOff = coupOn != null ? coupOn.PercentOff : null,
                    AmountOff = coupOn != null ? coupOn.AmountOff : null
                };

                response.Data = dashboard;
                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }


        public async Task<ResponseData<Dashboard>> UpdateGeoFenceTrip(string uuid, string lat, string lon)
        {
            try
            {
                var response = new ResponseData<Dashboard>();

                var lAccountAppliance = _accountApplianceService.GetAccountApplianceByUUID(uuid);

                //get all appliance by uuid
                var listAppliance = _accountApplianceService.GetAllApplianceOfUUID(uuid);

                if (listAppliance != null && listAppliance.Count() > 0)
                {
                    foreach (var item in listAppliance)
                    {
                        if (item.GeoFenceEnabled == false && item.TimerEnabled == false)
                            continue;

                        bool isTurnOnNetwork = false;

                        if (item.GeoFenceEnabled.Value)
                        {
                            if(lAccountAppliance != null && lAccountAppliance.Count > 0)
                            {
                                foreach (var accountAppliance in lAccountAppliance)
                                {
                                    if(accountAppliance.ApplianceId == item.Id)
                                    {
                                        accountAppliance.Lat = Convert.ToDouble(lat);
                                        accountAppliance.Lon = Convert.ToDouble(lon);
                                        _accountApplianceService.Update(accountAppliance);
                                    }
                                }
                            }
                            var lAccounts = _accountApplianceService.GetAccountByApplianceId(item.Id, true);
                            //lAccounts = lAccounts.Where(x => x.Id != item.Id).ToList();

                            foreach (var account in lAccounts)
                            {
                                if (!isTurnOnNetwork)
                                {
                                    double lon1 = item.Lon != null && item.Lon.HasValue ? item.Lon.Value : 0;
                                    double lat1 = item.Lat != null && item.Lat.HasValue ? item.Lat.Value : 0;
                                    double lon2 = account.Lon != null && account.Lon.HasValue ? account.Lon.Value : 0;
                                    double lat2 = account.Lat != null && account.Lat.HasValue ? account.Lat.Value : 0;
                                    var isMatching = DistanceBetweenPlaces(lon1, lat1, lon2, lat2);
                                    if (isMatching < Convert.ToDouble(item.TriggerMile))
                                    {
                                        isTurnOnNetwork = true;
                                    }

                                }
                            }

                            //get appliance in telit
                            string applianceEnvironment = string.Empty;
                            var thingFind = await TelitApi.ThingFind(item.SerialNumber);
                            Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                            bool? wifi = null;

                            if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                            {
                                _params = thingFind.Things.@params;
                            }

                            if (_params != null && _params.alarms != null && _params.alarms.env != null && _params.alarms.env.state >= 0 && _params.alarms.env.state < 16)
                            {
                                applianceEnvironment = Convert.ToString(_params.alarms.env.state, 2).PadLeft(4, '0');
                                var array = !string.IsNullOrEmpty(applianceEnvironment) ? applianceEnvironment.ToArray() : new char[] { };

                                wifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;

                            }

                            if (!string.IsNullOrEmpty(applianceEnvironment) && wifi != null && wifi.HasValue)
                            {
                                if (wifi.Value != isTurnOnNetwork)
                                {
                                    if (isTurnOnNetwork)
                                    {
                                        var updateTimerState = await TelitApi.UpdateTimerState(item.SerialNumber, Configuration.TimerEnable, true);
                                        string message = "Update from Geofence";
                                        await TelitApi.UpdateAlarmState(item.SerialNumber, AlarmStatus.OnGeofence, message);
                                        var UpdateNetwork = await TelitApi.UpdateEnvironment(item.SerialNumber, Convert.ToInt32(ReplaceAt(applianceEnvironment, 1, '1'), 2), Constant.EventType.StatusChangeFromGeoFence.ToString());
                                        var isSuccess = await TelitApi.UpdateAlarmState(item.SerialNumber, AlarmStatus.OnGeofence, string.Empty);
                                    }
                                    else
                                    {
                                        var updateTimerState = await TelitApi.UpdateTimerState(item.SerialNumber, Configuration.TimerDisable, false);
                                        string message = "Update from Geofence";
                                        await TelitApi.UpdateAlarmState(item.SerialNumber, AlarmStatus.OffGeofence, message);
                                        var UpdateNetwork = await TelitApi.UpdateEnvironment(item.SerialNumber, Convert.ToInt32(ReplaceAt(applianceEnvironment, 1, '0'), 2), Constant.EventType.StatusChangeFromGeoFence.ToString());
                                        var isSuccess = await TelitApi.UpdateAlarmState(item.SerialNumber, AlarmStatus.OffGeofence, string.Empty);
                                    }
                                }
                            }
                        }
                    }
                }

                response.Status = ResponseStatus.Success.ToString();
                return response;

            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }

        }

        public ResponseData<Dashboard> AddPhoneNumber(string uuid, string phoneNumber, string deviceToken)
        {
            try
            {
                var account = _accountService.GetAccountByPhoneNumber(phoneNumber);
                if (account != null)
                {
                    var listAccountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdForUpdate(account.Id);
                    foreach (var item in listAccountAppliance)
                    {
                        item.IdentifierForVendor = uuid;
                        item.DeviceToken = deviceToken;
                        _accountApplianceService.Update(item);
                    }
                }
                else
                {
                    account = new Account()
                    {
                        PhoneNumber = phoneNumber,
                        FirstName = Configuration.Unknown,
                        LastName = Configuration.Unknown,
                        Email = Configuration.Unknown,
                        IsVerified = false,
                        IsAdmin = false
                    };

                    _accountService.Insert(account);
                }

                var response = new ResponseData<Dashboard>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<Dashboard> CancelSubscription(string applianceId, string accountId)
        {
            try
            {
                var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(Convert.ToInt32(accountId), Convert.ToInt32(applianceId));
                var response = new ResponseData<Dashboard>();
                if(accountAppliance != null)
                {
                    var subscription = _stripeService.CancelSubscription(accountAppliance.SubscriptionId);
                    if(subscription != null)
                    {
                        response.Message = ResponseMessage.CancelSubscriptionSuccess;
                        var appliance = _applianceService.GetApplianceById(Convert.ToInt32(applianceId));
                        
                    }
                }

                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Dashboard>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }
    }
}
