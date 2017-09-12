using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Entity;
using Airgap.Entity.Entities;
using Airgap.Constant;
using Airgap.Telit;
using Microsoft.Extensions.Options;
using Airgap.Service;
using System.Net.Http;
using Airgap.WebApi.Helper;
using Newtonsoft.Json;

namespace Airgap.WebApi.Controllers
{
    public class TelitGatewayController : BaseController
    {
        private IApplianceService _applianceService;
        private IAccountApplianceService _accountApplianceService;
        private IEventService _eventService;
        private INotificationService _notificationService;
        private INotificationPreferenceService _notificationPreferenceService;
        private IStateService _stateService;
        private ITimerScheduleService _timerScheduleService;

        public TelitGatewayController(IStateService stateService, ITimerScheduleService timerScheduleService, IAccountApplianceService accountApplianceService, INotificationPreferenceService notificationPreferenceService, INotificationService notificationService, IEventService eventService, IApplianceService applianceService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._stateService = stateService;
            this._accountApplianceService = accountApplianceService;
            this._applianceService = applianceService;
            this._eventService = eventService;
            this._notificationService = notificationService;
            this._notificationPreferenceService = notificationPreferenceService;
            this._timerScheduleService = timerScheduleService;
        }

        public async Task<ResponseData<Telit.ThingList.RootObject>> Index()
        {
            var things = await TelitApi.ThingList();

            var response = new ResponseData<Telit.ThingList.RootObject>();
            response.Data = things;
            response.Message = "Ok";
            response.Status = "1";
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusFromTelit([FromBody] RequestTelit request)
        {
            try
            {
                Event _event = null;
                var appliance = _applianceService.GetApplianceBySerialNumber(request.thing);
                if (appliance == null)
                {
                    return Json(new { success = true, message = ResponseMessage.SerialNumberInCorrect });
                }

                if (request.key.ToLower() == Configuration.TurnOn)
                {
                    
                    if (appliance != null)
                    {

                        var thingFind = await TelitApi.ThingFind(request.thing);
                        Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                        bool isMatching = false;
                        if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                        {
                            _params = thingFind.Things.@params;
                            if (_params.loc != null)
                            {
                                isMatching = DistanceBetweenPlaces(appliance.Lat.Value, appliance.Lon.Value, _params.loc.lat, _params.loc.lng) <= Convert.ToDouble(_appSettings.Miles) ? true : false;
                                if (!isMatching)
                                {
                                    appliance.StateId = _params.loc != null && _params.loc.addr != null && _stateService.GetStateByNameOrCode(_params.loc.addr.state) != null ? (long?)_stateService.GetStateByNameOrCode(_params.loc.addr.state).Id : null;
                                    appliance.Street1 = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.streetNumber + " " + _params.loc.addr.street : string.Empty;
                                    appliance.Street2 = string.Empty;
                                    appliance.Lat = _params.loc != null ? _params.loc.lat : 0;
                                    appliance.Lon = _params.loc != null ? _params.loc.lng : 0;
                                    appliance.City = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.city : string.Empty;
                                    appliance.ZipCode = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.zipCode : string.Empty;
                                }
                            }
                        }

                        appliance.IsOn = Convert.ToBoolean(Convert.ToInt16(request.value));
                        if (Convert.ToInt16(request.value) == Configuration.Off)
                        {
                            appliance.TimerEnabled = true;
                            appliance.GeoFenceEnabled = true;
                        }
                        _applianceService.Update(appliance);

                        if (Convert.ToInt32(request.value) == (Int32)AlarmStatus.OffScheduleTimer || Convert.ToInt32(request.value) == (Int32)AlarmStatus.OnScheduleTimer)
                        {
                            request.msg = ResponseMessage.ScheduleTimerChange;
                        }

                        _event = new Event()
                        {
                            ApplianceId = Convert.ToInt64(appliance.Id),
                            EventTypeId = Constant.EventType.ConnectionChange,
                            EventDetail = Constant.ResponseMessage.ConnectionChange,
                            Timestamp = DateTime.UtcNow,
                            Message = request.msg
                        };

                        _eventService.Insert(_event);

                    }

                }
                else if (request.key.ToLower() == Configuration.Name)
                {
                    if (appliance != null)
                    {
                        appliance.DeviceName = request.value;
                        _applianceService.Update(appliance);
                    }
                }
                else if (request.key.ToLower() == Configuration.Env)
                {
                    var applianceEnvironment = Convert.ToString(Convert.ToInt16(request.value), 2).PadLeft(4, '0');
                    var array = !string.IsNullOrEmpty(applianceEnvironment) ? applianceEnvironment.ToArray() : new char[] { };
                    if (appliance != null)
                    {
                        bool isPowerStatusChange = appliance.Power != (array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false) ? true : false;
                        bool isWifiStatusChange = appliance.Wifi != (array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false) ? true : false;
                        bool isWiFiInternetStatusChange = appliance.WiFiInternet != (array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false) ? true : false;
                        appliance.Cellular = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[2].ToString())) : false;
                        appliance.Power = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false;
                        appliance.Wifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;
                        appliance.WiFiInternet = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false;

                        var thingFind = await TelitApi.ThingFind(request.thing);
                        Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                        bool isMatching = false;
                        if (thingFind != null && thingFind.Things != null && thingFind.Things.success && thingFind.Things.@params != null)
                        {
                            _params = thingFind.Things.@params;
                            if (_params.loc != null)
                            {
                                isMatching = DistanceBetweenPlaces(appliance.Lat.Value, appliance.Lon.Value, _params.loc.lat, _params.loc.lng) <= Convert.ToDouble(_appSettings.Miles) ? true : false;
                                if (!isMatching)
                                {
                                    appliance.StateId = _params.loc != null && _params.loc.addr != null && _stateService.GetStateByNameOrCode(_params.loc.addr.state) != null ? (long?)_stateService.GetStateByNameOrCode(_params.loc.addr.state).Id : null;
                                    appliance.Street1 = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.streetNumber + " " + _params.loc.addr.street : string.Empty;
                                    appliance.Street2 = string.Empty;
                                    appliance.Lat = _params.loc != null ? _params.loc.lat : 0;
                                    appliance.Lon = _params.loc != null ? _params.loc.lng : 0;
                                    appliance.City = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.city : string.Empty;
                                    appliance.ZipCode = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.zipCode : string.Empty;
                                }
                            }
                        }

                        _applianceService.Update(appliance);
                        var lAccount = _accountApplianceService.GetAccountByApplianceId(appliance.Id, true);

                        if (isPowerStatusChange)
                        {
                            _event = new Event()
                            {
                                ApplianceId = Convert.ToInt64(appliance.Id),
                                EventTypeId = Constant.EventType.HomePowerLoss,
                                EventDetail = Constant.ResponseMessage.ManualConnectedStatusChange,
                                Timestamp = DateTime.UtcNow,
                                Message = appliance.DeviceName + "(" + appliance.SerialNumber.Substring(appliance.SerialNumber.Length - 4) + ")" + ": " + Constant.ResponseMessage.HomePower + " " + (array.Length == 4 && Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) ? Configuration.TurnOn.ToUpper() : Configuration.TurnOff.ToUpper())
                            };

                            var insEvent = _eventService.Insert(_event);

                            if (lAccount != null && lAccount.Count > 0)
                            {
                                foreach (var item in lAccount)
                                {
                                    var notificationPreference = _notificationPreferenceService.GetNotificationPreferenceByAccountId(item.Id.Value);
                                    if (notificationPreference.Any(x => x.EventTypeId == Constant.EventType.HomePowerLoss && x.ApplianceId == appliance.Id))
                                    {
                                        var temp = notificationPreference.FirstOrDefault(x => x.EventTypeId == Constant.EventType.HomePowerLoss && x.ApplianceId == appliance.Id);

                                        var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(temp.AccountId, temp.ApplianceId);
                                        if (accountAppliance.PhoneType == Configuration.Android)
                                        {
                                            var pushGcmotification = new AndroidGcmPushNotification(_appSettings);
                                            pushGcmotification.InitPushNotification(accountAppliance.DeviceToken, insEvent.Message);
                                            System.Threading.Thread.Sleep(1000);
                                        }
                                        else
                                        {
                                            var applePush = new ApplePushNotificationService(_appSettings);
                                            applePush.PushMessage(insEvent.Message, accountAppliance.DeviceToken, 0, null).Wait();
                                            System.Threading.Thread.Sleep(1000);
                                        }

                                        var notification = new Notification()
                                        {
                                            AccountId = accountAppliance.AccountId,
                                            ApplianceId = accountAppliance.ApplianceId,
                                            EventId = insEvent.Id,
                                            Timestamp = DateTime.UtcNow
                                        };

                                        _notificationService.Insert(notification);
                                    }
                                }
                            }
                        }

                        if (isWifiStatusChange)
                        {
                            _event = new Event()
                            {
                                ApplianceId = Convert.ToInt64(appliance.Id),
                                EventTypeId = request.msg != null && request.msg == Constant.EventType.StatusChangeFromGeoFence.ToString() ? Constant.EventType.StatusChangeFromGeoFence : Constant.EventType.NetWorkStatusChange,
                                EventDetail = Constant.ResponseMessage.Wifi,
                                Timestamp = DateTime.UtcNow,
                                Message = appliance.DeviceName + "(" + appliance.SerialNumber.Substring(appliance.SerialNumber.Length - 4) + ")" + ": " + Constant.ResponseMessage.Wifi + " " + (array.Length == 4 && Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) ? Configuration.TurnOn.ToUpper() : Configuration.TurnOff.ToUpper())
                            };

                            var insEvent = _eventService.Insert(_event);

                            if (lAccount != null && lAccount.Count > 0)
                            {
                                foreach (var item in lAccount)
                                {
                                    var notificationPreference = _notificationPreferenceService.GetNotificationPreferenceByAccountId(item.Id.Value);
                                    if (notificationPreference.Any(x => x.EventTypeId == Constant.EventType.NetWorkStatusChange && x.ApplianceId == appliance.Id))
                                    {
                                        var temp = notificationPreference.FirstOrDefault(x => x.EventTypeId == Constant.EventType.NetWorkStatusChange && x.ApplianceId == appliance.Id);
                                        var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(temp.AccountId, temp.ApplianceId);
                                        if (accountAppliance.PhoneType == Configuration.Android)
                                        {
                                            var pushGcmotification = new AndroidGcmPushNotification(_appSettings);
                                            pushGcmotification.InitPushNotification(accountAppliance.DeviceToken, insEvent.Message);
                                            System.Threading.Thread.Sleep(1000);
                                        }
                                        else
                                        {
                                            var applePush = new ApplePushNotificationService(_appSettings);
                                            applePush.PushMessage(insEvent.Message, accountAppliance.DeviceToken, 0, null).Wait();
                                            System.Threading.Thread.Sleep(1000);
                                        }

                                        var notification = new Notification()
                                        {
                                            AccountId = accountAppliance.AccountId,
                                            ApplianceId = accountAppliance.ApplianceId,
                                            EventId = insEvent.Id,
                                            Timestamp = DateTime.UtcNow
                                        };

                                        _notificationService.Insert(notification);
                                    }
                                }
                            }
                        }

                        if (isWiFiInternetStatusChange)
                        {
                            _event = new Event()
                            {
                                ApplianceId = Convert.ToInt64(appliance.Id),
                                EventTypeId = Constant.EventType.ISPOutage,
                                EventDetail = Constant.ResponseMessage.ISPOutage,
                                Timestamp = DateTime.UtcNow,
                                Message = appliance.DeviceName + "(" + appliance.SerialNumber.Substring(appliance.SerialNumber.Length - 4) + ")" + ": " + Constant.ResponseMessage.ISPOutage + " " + (array.Length == 4 && Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) ? Configuration.Connected : Configuration.Disconnected)
                            };

                            var insEvent = _eventService.Insert(_event);

                            if (lAccount != null && lAccount.Count > 0)
                            {
                                foreach (var item in lAccount)
                                {
                                    var notificationPreference = _notificationPreferenceService.GetNotificationPreferenceByAccountId(item.Id.Value);
                                    if (notificationPreference.Any(x => x.EventTypeId == Constant.EventType.ISPOutage && x.ApplianceId == appliance.Id))
                                    {
                                        var temp = notificationPreference.FirstOrDefault(x => x.EventTypeId == Constant.EventType.ISPOutage && x.ApplianceId == appliance.Id);
                                        var accountAppliance = _accountApplianceService.GetAccountApplianceByAccountIdAndApplianceId(temp.AccountId, temp.ApplianceId);
                                        if (accountAppliance.PhoneType == Configuration.Android)
                                        {
                                            var pushGcmotification = new AndroidGcmPushNotification(_appSettings);
                                            pushGcmotification.InitPushNotification(accountAppliance.DeviceToken, insEvent.Message);
                                            System.Threading.Thread.Sleep(1000);
                                        }
                                        else
                                        {
                                            var applePush = new ApplePushNotificationService(_appSettings);
                                            applePush.PushMessage(insEvent.Message, accountAppliance.DeviceToken, 0, null).Wait();
                                            System.Threading.Thread.Sleep(1000);
                                        }

                                        var notification = new Notification()
                                        {
                                            AccountId = accountAppliance.AccountId,
                                            ApplianceId = accountAppliance.ApplianceId,
                                            EventId = insEvent.Id,
                                            Timestamp = DateTime.UtcNow
                                        };

                                        _notificationService.Insert(notification);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (request.key.ToLower() == Configuration.Trust.ToLower())
                {
                    if (appliance != null)
                    {
                        bool isStatusChange = appliance.TrustLevel != null && appliance.TrustLevel.Value.ToString() != request.value ? true : false;
                        _event = new Event()
                        {
                            ApplianceId = Convert.ToInt64(appliance.Id),
                            EventTypeId = Constant.EventType.TrustLevelChange,
                            EventDetail = Constant.ResponseMessage.TrustLevel,
                            Timestamp = DateTime.UtcNow,
                            Message = Constant.ResponseMessage.TrustLevel + " " + request.value
                        };

                        _eventService.Insert(_event);
                        appliance.TrustLevel = Convert.ToInt16(request.value);
                        _applianceService.Update(appliance);
                    }
                }
                else if (request.key.ToLower() == Configuration.TimmerState)
                {
                    if (appliance != null)
                    {

                        appliance.TimerEnabled = Convert.ToInt16(request.value) != Configuration.TimerDisable ? true : false;
                        _applianceService.Update(appliance);
                    }
                }
                else if (request.key.ToLower() == Configuration.TimerSchedule)
                {
                    var timerScheduleDTO = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(request.value);
                    var timerScheduleOldValue = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(request.prev);
                    char[] weekDayArray = timerScheduleDTO.Weekdays.ToCharArray();
                    char[] weekEndArray = timerScheduleDTO.Weekends.ToCharArray();
                    string valueWeekDay = string.Empty;
                    string valueWeekEnd = string.Empty;

                    for (int i = 0; i < weekDayArray.Length; i++)
                    {
                        if (weekDayArray[i] == '1')
                        {
                            valueWeekDay += i + 1 + ",";
                        }
                    }

                    for (int i = 0; i < weekEndArray.Length; i++)
                    {
                        if (weekEndArray[i] == '1')
                        {
                            valueWeekEnd += i + 1 + ",";
                        }
                    }

                    var timerSchedule = _timerScheduleService.GetTimerScheduleByApplianceId(appliance.Id);
                    if (timerSchedule != null && timerSchedule.Count > 0)
                    {
                        foreach (var item in timerSchedule)
                        {

                            if (item.TimerTypeId == Configuration.Weekdays)
                            {
                                item.ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty;
                            }

                            if (item.TimerTypeId == Configuration.Weekends)
                            {
                                item.ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty;
                            }
                            _timerScheduleService.Update(item);

                        }

                        _event = new Event()
                        {
                            ApplianceId = Convert.ToInt64(appliance.Id),
                            EventTypeId = Constant.EventType.ScheduleTimerChange,
                            EventDetail = Constant.ResponseMessage.ScheduleTimerChange,
                            Timestamp = DateTime.UtcNow,
                            Message = "from Weekdays: " + timerScheduleOldValue.Weekdays + " Weekends: " + timerScheduleOldValue.Weekends + " to Weekdays: " + timerScheduleDTO.Weekdays + " Weekends: " + timerScheduleDTO.Weekends
                        };

                        _eventService.Insert(_event);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(valueWeekDay))
                        {
                            var timerScheduleOfWeekDays = new TimerSchedule()
                            {
                                TimerTypeId = Configuration.Weekdays,
                                ApplianceId = appliance.Id,
                                ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty
                            };
                            _timerScheduleService.Insert(timerScheduleOfWeekDays);
                        }

                        if (!string.IsNullOrEmpty(valueWeekEnd))
                        {
                            var timerScheduleOfWeekEnd = new TimerSchedule()
                            {
                                TimerTypeId = Configuration.Weekends,
                                ApplianceId = appliance.Id,
                                ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty
                            };
                            _timerScheduleService.Insert(timerScheduleOfWeekEnd);
                        }

                        _event = new Event()
                        {
                            ApplianceId = Convert.ToInt64(appliance.Id),
                            EventTypeId = Constant.EventType.ScheduleTimerChange,
                            EventDetail = Constant.ResponseMessage.ScheduleTimerChange,
                            Timestamp = DateTime.UtcNow,
                            Message = "from Weekdays: " + timerScheduleOldValue.Weekdays + " Weekends: " + timerScheduleOldValue.Weekends + " to Weekdays: " + timerScheduleDTO.Weekdays + " Weekends: " + timerScheduleDTO.Weekends
                        };

                        _eventService.Insert(_event);

                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = true, message = request.msg });
        }

    }
}
