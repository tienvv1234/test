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
using System.Net;
using System.Xml.Linq;
using Airgap.Service.Helper;
using Airgap.WebApi.Helper;
using System.Text;
using Newtonsoft.Json;

namespace Airgap.WebApi.Controllers
{
    public class SettingController : BaseController
    {
        private IApplianceService _applianceService;
        private ITimerScheduleService _timerScheduleService;
        private IAccountApplianceService _accountApplianceService;
        private IStateService _stateService;
        private IHelperService _helperService;
        public SettingController(IHelperService helperService, ITimerScheduleService timerScheduleService, IAccountApplianceService accountApplianceService, IApplianceService applianceService, IStateService stateService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._helperService = helperService;
            this._applianceService = applianceService;
            this._stateService = stateService;
            this._accountApplianceService = accountApplianceService;
            this._timerScheduleService = timerScheduleService;
        }
        public ResponseData<Settings> Index(string id)
        {
            try
            {
                var states = _stateService.GetAllState();
                var appliances = _applianceService.GetApplianceByAccountId(Convert.ToInt64(id));
                var settings = new Settings()
                {
                    Appliances = appliances,
                    States = states
                };

                var response = new ResponseData<Settings>();
                response.Data = settings;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Settings>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public async Task<ResponseData<ApplianceDTO>> SetupThink([FromBody]ApplianceDTO appliance)
        {
            try
            {
                var response = new ResponseData<ApplianceDTO>();
                var things = await TelitApi.ThingList();
                bool isMatching = false;
                bool hasSerialNumber = false;
                Telit.ThingList.Result thing = new Telit.ThingList.Result();
                if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                {
                    foreach (var item in things.things.@params.result)
                    {
                        if (item.key == appliance.SerialNumber)
                        {
                            hasSerialNumber = true;
                            if (item.loc != null && item.loc.addr != null)
                            {
                                isMatching = await calculateAddressAsync(appliance.Street1.Trim(), appliance.City.Trim(), _stateService.GetStateById(appliance.StateId.Value).Name, appliance.ZipCode.Trim(), item.loc.lat, item.loc.lng);
                                thing = item;
                                break;
                            }
                        }
                    }
                }

                if (!hasSerialNumber)
                {
                    response.Message = ResponseMessage.SerialNumberInCorrect;
                    response.Status = ResponseStatus.Success.ToString();
                    return response;
                }

                if (isMatching)
                {
                    var isSerialNumberExist = _applianceService.GetApplianceBySerialNumber(appliance.SerialNumber);

                    var thingFind = await TelitApi.ThingFind(thing.key);
                    bool thingIsOn = false;
                    bool thingIsConnected = false;
                    bool thingCellular = false;
                    bool thingPower = false;
                    bool thingWifi = false;
                    bool thingWifiInternet = false;
                    int? thingTrustLevel = null;
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
                        thingIsOn = Convert.ToBoolean(_params.alarms.on.state);
                        thingIsConnected = _params.connected;
                        thingCellular = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[2].ToString())) : false;
                        thingPower = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false;
                        thingWifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;
                        thingWifiInternet = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false;
                        thingTrustLevel = _params.alarms.trust != null ? (int?)_params.alarms.trust.state : null;
                    }

                    if (isSerialNumberExist == null)
                    {

                        var app = _applianceService.Insert(new Appliance()
                        {
                            AccountId = appliance.AccountId,
                            Cellular = thingCellular,
                            IsConnected = thingIsConnected,
                            City = appliance.City,
                            DeviceName = _params != null && _params.attrs != null && _params.attrs.name != null ? _params.attrs.name.value : string.Empty,
                            OsVersion = "",
                            GeoFenceEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false,
                            Lat = _params.loc != null ? _params.loc.lat : 0,
                            Lon = _params.loc != null ? _params.loc.lng : 0,
                            SerialNumber = appliance.SerialNumber,
                            Power = thingPower,
                            StateId = appliance.StateId,
                            Street1 = appliance.Street1,
                            Street2 = appliance.Street2,
                            TimerEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false,
                            TriggerMile = Convert.ToDecimal(_appSettings.TriggerMiles),
                            Wifi = thingWifi,
                            WiFiInternet = thingWifiInternet,
                            ZipCode = appliance.ZipCode,
                            TrustLevel = thingTrustLevel != null ? thingTrustLevel : 0,
                            IsOn = thingIsOn
                        });
                        if (app != null)
                        {
                            var accountAppliance = _accountApplianceService.Insert(new AccountAppliance()
                            {
                                AccountId = app.AccountId,
                                AirGapVersion = "",
                                ApplianceId = app.Id,
                                IsQRCodeScaned = false,
                                IsVerified = false
                            });

                            appliance.Id = app.Id;

                            if (_params.attrs != null && _params.attrs.timerSchedule != null)
                            {
                                var timerScheduleDTO = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(_params.attrs.timerSchedule.value);
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

                                if (!string.IsNullOrEmpty(valueWeekDay))
                                {
                                    var timerScheduleOfWeekDays = new TimerSchedule()
                                    {
                                        TimerTypeId = Configuration.Weekdays,
                                        ApplianceId = app.Id,
                                        ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty
                                };
                                    _timerScheduleService.Insert(timerScheduleOfWeekDays);
                                }

                                if (!string.IsNullOrEmpty(valueWeekEnd))
                                {
                                    var timerScheduleOfWeekEnd = new TimerSchedule()
                                    {
                                        TimerTypeId = Configuration.Weekends,
                                        ApplianceId = app.Id,
                                        ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty
                                    };

                                    _timerScheduleService.Insert(timerScheduleOfWeekEnd);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isSerialNumberExist.AccountId == null)
                        {
                            isSerialNumberExist.AccountId = appliance.AccountId;
                            isSerialNumberExist.Street1 = appliance.Street1;
                            isSerialNumberExist.Street2 = appliance.Street2;
                            isSerialNumberExist.City = appliance.City;
                            isSerialNumberExist.StateId = appliance.StateId;
                            isSerialNumberExist.ZipCode = appliance.ZipCode;
                            isSerialNumberExist.Lat = _params.loc != null ? _params.loc.lat : 0;
                            isSerialNumberExist.Lon = _params.loc != null ? _params.loc.lng : 0;
                            isSerialNumberExist.Cellular = thingCellular;
                            isSerialNumberExist.IsConnected = thingIsConnected;
                            isSerialNumberExist.DeviceName = _params != null && _params.attrs != null && _params.attrs.name != null ? _params.attrs.name.value : string.Empty;
                            isSerialNumberExist.OsVersion = "";
                            isSerialNumberExist.GeoFenceEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false;
                            isSerialNumberExist.Power = thingPower;
                            isSerialNumberExist.TimerEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false;
                            isSerialNumberExist.Wifi = thingWifi;
                            isSerialNumberExist.WiFiInternet = thingWifiInternet;
                            isSerialNumberExist.ZipCode = appliance.ZipCode;
                            isSerialNumberExist.TrustLevel = thingTrustLevel != null ? thingTrustLevel : 0;
                            isSerialNumberExist.IsOn = thingIsOn;
                            isSerialNumberExist.TriggerMile = Convert.ToDecimal(_appSettings.TriggerMiles);
                            _applianceService.Update(isSerialNumberExist);
                            var accountAppliance = _accountApplianceService.Insert(new AccountAppliance()
                            {
                                AccountId = appliance.AccountId,
                                AirGapVersion = "",
                                ApplianceId = isSerialNumberExist.Id,
                                IsQRCodeScaned = false,
                                IsVerified = false
                            });
                            appliance.Id = isSerialNumberExist.Id;

                            if (_params.attrs != null && _params.attrs.timerSchedule != null)
                            {
                                var timerScheduleDTO = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(_params.attrs.timerSchedule.value);
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
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(valueWeekDay))
                                    {
                                        var timerScheduleOfWeekDays = new TimerSchedule()
                                        {
                                            TimerTypeId = Configuration.Weekdays,
                                            ApplianceId = isSerialNumberExist.Id,
                                            ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty
                                        };
                                        _timerScheduleService.Insert(timerScheduleOfWeekDays);
                                    }

                                    if (!string.IsNullOrEmpty(valueWeekEnd))
                                    {
                                        var timerScheduleOfWeekEnd = new TimerSchedule()
                                        {
                                            TimerTypeId = Configuration.Weekends,
                                            ApplianceId = isSerialNumberExist.Id,
                                            ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty
                                        };

                                        _timerScheduleService.Insert(timerScheduleOfWeekEnd);
                                    }

                                }
                            }
                        }
                        else
                        {
                            response.Status = ResponseStatus.Success.ToString();
                            response.Message = ResponseMessage.SerialNumberIsExist;
                            return response;
                        }
                    }
                }

                appliance.IsMatching = isMatching;
                appliance.HasSerialNumber = hasSerialNumber;
                response.Data = appliance;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<ApplianceDTO>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public async Task<ResponseData<ApplianceDTO>> UseAddressOfTelit([FromBody]ApplianceDTO appliance)
        {
            try
            {
                var response = new ResponseData<ApplianceDTO>();
                var things = await TelitApi.ThingList();
                Telit.ThingList.Result thing = new Telit.ThingList.Result();
                if (things != null && things.things != null && things.things.success && things.things.@params != null && things.things.@params.result != null && things.things.@params.result.Count() > 0)
                {
                    foreach (var item in things.things.@params.result)
                    {
                        if (item.key == appliance.SerialNumber)
                        {
                            thing = item;
                            break;
                        }
                    }
                }

                var thingFind = await TelitApi.ThingFind(thing.key);
                bool thingIsOn = false;
                bool thingIsConnected = false;
                bool thingCellular = false;
                bool thingPower = false;
                bool thingWifi = false;
                bool thingWifiInternet = false;
                int? thingTrustLevel = null;
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
                    thingIsOn = _params.alarms.on != null ? Convert.ToBoolean(_params.alarms.on.state) : false;
                    thingIsConnected = _params.connected;
                    thingCellular = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[2].ToString())) : false;
                    thingPower = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[3].ToString())) : false;
                    thingWifi = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[1].ToString())) : false;
                    thingWifiInternet = array.Length == 4 ? Convert.ToBoolean(Convert.ToInt16(array[0].ToString())) : false;
                }

                if (_params != null && _params.alarms != null)
                {
                    thingTrustLevel = _params.alarms.trust != null ? (int?)_params.alarms.trust.state : 3;
                }

                var isSerialNumberExist = _applianceService.GetApplianceBySerialNumber(appliance.SerialNumber);
                if (isSerialNumberExist == null)
                {


                    var app = _applianceService.Insert(new Appliance()
                    {
                        AccountId = appliance.AccountId,
                        Cellular = thingCellular,
                        IsConnected = thingIsConnected,
                        IsOn = thingIsOn,
                        City = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.city : string.Empty,
                        DeviceName = _params != null && _params.attrs != null && _params.attrs.name != null ? _params.attrs.name.value : string.Empty,
                        OsVersion = "",
                        GeoFenceEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false,
                        Lat = _params.loc != null ? _params.loc.lat : 0,
                        Lon = _params.loc != null ? _params.loc.lng : 0,
                        SerialNumber = appliance.SerialNumber,
                        Power = thingPower,
                        StateId = _params.loc != null && _params.loc.addr != null && _stateService.GetStateByNameOrCode(_params.loc.addr.state) != null ? (long?)_stateService.GetStateByNameOrCode(_params.loc.addr.state).Id : null,
                        Street1 = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.streetNumber + " " + _params.loc.addr.street : string.Empty,
                        Street2 = string.Empty,
                        TimerEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false,
                        TriggerMile = Convert.ToDecimal(_appSettings.TriggerMiles),
                        Wifi = thingWifi,
                        WiFiInternet = thingWifiInternet,
                        ZipCode = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.zipCode : string.Empty,
                        TrustLevel = thingTrustLevel != null ? thingTrustLevel : 0,
                    });
                    if (app != null)
                    {
                        var accountAppliance = _accountApplianceService.Insert(new AccountAppliance()
                        {
                            AccountId = app.AccountId,
                            AirGapVersion = "",
                            ApplianceId = app.Id,
                            IsQRCodeScaned = false,
                            IsVerified = false
                        });
                        appliance.Id = app.Id;

                        if (_params.attrs != null && _params.attrs.timerSchedule != null)
                        {
                            var timerScheduleDTO = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(_params.attrs.timerSchedule.value);
                            char[] weekDayArray = timerScheduleDTO.Weekdays.ToCharArray();
                            char[] weekEndArray = timerScheduleDTO.Weekends.ToCharArray();
                            string valueWeekDay = string.Empty;
                            string valueWeekEnd = string.Empty;

                            for(int i = 0; i < weekDayArray.Length; i++)
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

                            if (!string.IsNullOrEmpty(valueWeekDay))
                            {
                                var timerScheduleOfWeekDays = new TimerSchedule()
                                {
                                    TimerTypeId = Configuration.Weekdays,
                                    ApplianceId = app.Id,
                                    ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty
                                };
                                _timerScheduleService.Insert(timerScheduleOfWeekDays);
                            }

                            if (!string.IsNullOrEmpty(valueWeekEnd))
                            {
                                var timerScheduleOfWeekEnd = new TimerSchedule()
                                {
                                    TimerTypeId = Configuration.Weekends,
                                    ApplianceId = app.Id,
                                    ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty
                            };

                                _timerScheduleService.Insert(timerScheduleOfWeekEnd);
                            }
                        }
                    }
                }
                else
                {
                    if (isSerialNumberExist.AccountId == null)
                    {
                        isSerialNumberExist.AccountId = appliance.AccountId;
                        isSerialNumberExist.Street1 = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.streetNumber + " " + _params.loc.addr.street : string.Empty;
                        isSerialNumberExist.Street2 = string.Empty;
                        isSerialNumberExist.City = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.city : string.Empty;
                        isSerialNumberExist.StateId = _params.loc != null && _params.loc.addr != null && _stateService.GetStateByNameOrCode(_params.loc.addr.state) != null ? (long?)_stateService.GetStateByNameOrCode(_params.loc.addr.state).Id : null;
                        isSerialNumberExist.ZipCode = _params.loc != null && _params.loc.addr != null ? _params.loc.addr.zipCode : string.Empty;
                        isSerialNumberExist.Lat = _params.loc != null ? _params.loc.lat : 0;
                        isSerialNumberExist.Lon = _params.loc != null ? _params.loc.lng : 0;
                        isSerialNumberExist.Cellular = thingCellular;
                        isSerialNumberExist.IsConnected = thingIsConnected;
                        isSerialNumberExist.DeviceName = _params != null && _params.attrs != null && _params.attrs.name != null ? _params.attrs.name.value : string.Empty;
                        isSerialNumberExist.OsVersion = "";
                        isSerialNumberExist.GeoFenceEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false;
                        isSerialNumberExist.Power = thingPower;
                        isSerialNumberExist.TimerEnabled = _params.alarms != null && _params.alarms.timerState != null && _params.alarms.timerState.state != 0 ? true : false;
                        isSerialNumberExist.Wifi = thingWifi;
                        isSerialNumberExist.WiFiInternet = thingWifiInternet;
                        isSerialNumberExist.TrustLevel = thingTrustLevel != null ? thingTrustLevel : 0;
                        isSerialNumberExist.IsOn = thingIsOn;
                        isSerialNumberExist.TriggerMile = Convert.ToDecimal(_appSettings.TriggerMiles);
                        _applianceService.Update(isSerialNumberExist);
                        var accountAppliance = _accountApplianceService.Insert(new AccountAppliance()
                        {
                            AccountId = appliance.AccountId,
                            AirGapVersion = "",
                            ApplianceId = isSerialNumberExist.Id,
                            IsQRCodeScaned = false,
                            IsVerified = false
                        });
                        appliance.Id = isSerialNumberExist.Id;

                        if (_params.attrs != null && _params.attrs.timerSchedule != null)
                        {
                            var timerScheduleDTO = JsonConvert.DeserializeObject<TimerScheduleFromTelit>(_params.attrs.timerSchedule.value);
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
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(valueWeekDay))
                                {
                                    var timerScheduleOfWeekDays = new TimerSchedule()
                                    {
                                        TimerTypeId = Configuration.Weekdays,
                                        ApplianceId = isSerialNumberExist.Id,
                                        ActiveValues = !string.IsNullOrEmpty(valueWeekDay) ? valueWeekDay.Remove(valueWeekDay.Length - 1) : string.Empty
                                };
                                    _timerScheduleService.Insert(timerScheduleOfWeekDays);
                                }

                                if (!string.IsNullOrEmpty(valueWeekEnd))
                                {
                                    var timerScheduleOfWeekEnd = new TimerSchedule()
                                    {
                                        TimerTypeId = Configuration.Weekends,
                                        ApplianceId = isSerialNumberExist.Id,
                                        ActiveValues = !string.IsNullOrEmpty(valueWeekEnd) ? valueWeekEnd.Remove(valueWeekEnd.Length - 1) : string.Empty
                                };

                                    _timerScheduleService.Insert(timerScheduleOfWeekEnd);
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        response.Status = ResponseStatus.Success.ToString();
                        response.Message = ResponseMessage.SerialNumberIsExist;
                        return response;
                    }
                }

                response.Data = appliance;
                response.Status = ResponseStatus.Success.ToString();
                response.Message = ResponseMessage.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<ApplianceDTO>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }
    }

    public class TimerScheduleFromTelit
    {
        public string Weekdays;
        public string Weekends;
    }
}