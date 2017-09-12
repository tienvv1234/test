using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Airgap.Constant;
using Airgap.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Airgap.Telit.ThingList;
using Airgap.Data.ApiEntities;
using Airgap.WebUi.Models;
using Airgap.Data.DTOEntities;
using Microsoft.AspNetCore.Http;
using Stripe;

namespace Airgap.WebUi.Controllers
{
    public class HomeController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        public HomeController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ActionResult> Index()
        {
            if (current_UserID <= 0)
            {
                return RedirectToAction("Signin", "Login");
            }
            else if (current_ApplianceId <= 0)
            {
                return RedirectToAction("Index", "Setup");
            }
            else
            {
                long applianceId = 0;
                if (current_ApplianceId > 0)
                {
                    applianceId = current_ApplianceId;
                }

                ViewBag.ActiveMenu = "dashboard";
                string url = _appSettings.ApiUrl + "/Home/Index";
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?accountid=" + current_UserID + "&applianceid=" + current_ApplianceId);

                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    DateTime timer = new DateTime();
                    List<ApplianceDTO> listAppliance = new List<ApplianceDTO>();
                    //ApplianceDTO selectedAppliance = null;
                    List<AccountApplianceDTO> lAccountApplianceDTO = new List<AccountApplianceDTO>();
                    if (responseData.Data != null && responseData.Data.AccountAppliance != null && responseData.Data.AccountAppliance.Count > 0)
                    {
                        foreach (var item in responseData.Data.AccountAppliance)
                        {
                            //if (item.ApplianceDTO.Id == applianceId)
                            //{
                            //    selectedAppliance = item.ApplianceDTO;
                            //}
                            listAppliance.Add(item.ApplianceDTO);
                        }
                    }

                    foreach (var item in listAppliance)
                    {
                        item.DeviceName += "(" + item.SerialNumber.Substring(item.SerialNumber.Length - 4) + ")";
                    }

                    DashBoardViewModels dashBroad = new DashBoardViewModels()
                    {
                        lAppliance = listAppliance,
                        lEvent = responseData.Data.Events != null && responseData.Data.Events.Count() > 0 ? responseData.Data.Events : null,
                        SelectedAppliance = responseData.Data.AppDTO,
                        lAcccountAppliance = responseData.Data.AccountAppliance != null && responseData.Data.AccountAppliance.Count() > 0 ? responseData.Data.AccountAppliance : null,
                        lAccountsDTO = responseData.Data.ListAccountsDTO != null && responseData.Data.ListAccountsDTO.Count > 0 ? responseData.Data.ListAccountsDTO : null,
                        Timer = timer != DateTime.MaxValue && timer != DateTime.MinValue ? (DateTime?)timer : null,
                        IOTIsConnected = responseData.Data.IsIOTConnected,
                        lPlan = responseData.Data.Plans,
                        IsApplianceConnected = responseData.Data.IsApplianceConnected,
                        ExpireDate = responseData.Data.ExpireDate,
                        CancelAtEnd = responseData.Data.CancelAtEnd
                    };

                    return View(dashBroad);
                }

                return View();
            }

        }

        public async Task<ActionResult> ChangeAppliance(string applianceId, string power)
        {
            string url = _appSettings.ApiUrl + "/Home/UpdateAppliance";
            client.BaseAddress = new Uri(url);

            if (applianceId != HttpContext.Session.GetString("ApplianceId"))
            {
                HttpContext.Session.SetString("ApplianceId", $"{applianceId}");
            }

            if (!string.IsNullOrEmpty(power))
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceid=" + applianceId + "&accountId=" + current_UserID + "&power=" + power);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;

                }
            }
            System.Threading.Thread.Sleep(2000);
            return RedirectToAction("Index", "Home", new { });
        }

        public async Task<ActionResult> SetAirGapOn(string cbxAirGapOnStatus)
        {
            string url = _appSettings.ApiUrl + "/Home/SetAirGapOn";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceid=" + HttpContext.Session.GetString("ApplianceId") + "&accountId=" + current_UserID + "&airGapAlwaysStatus=" + cbxAirGapOnStatus);
            var responseData = new ResponseData<Dashboard>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
            }
            System.Threading.Thread.Sleep(2000);
            return RedirectToAction("Index", "Home", new { });
        }

        public async Task<ActionResult> Filler(string dateFrom, string dateTo)
        {
            string url = _appSettings.ApiUrl + "/Home/Filler";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceid=" + HttpContext.Session.GetString("ApplianceId") + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo);
            var responseData = new ResponseData<Dashboard>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                ApplianceDTO selectedAppliance = null;
                if (responseData.Data != null && responseData.Data.ApplianceDTO != null && responseData.Data.ApplianceDTO.Count > 0)
                {
                    selectedAppliance = responseData.Data.ApplianceDTO.FirstOrDefault();
                }
                return Json(new
                {
                    success = true,
                    responseText = responseData.Message,
                    Data = responseData.Data.Events.Select(x => new object[] {
                x.Timestamp != null && x.Timestamp.HasValue ? x.Timestamp.Value.ToString("dd/MM/yyyy hh:mm:ss tt"):  null,
                x.Account != null ? (x.Account.IsAdmin != null && x.Account.IsAdmin.HasValue ? Configuration.Administrator : x.Account.PhoneNumber) : (selectedAppliance != null ? selectedAppliance.DeviceName + "(" + selectedAppliance.SerialNumber.Substring(selectedAppliance.SerialNumber.Length - 4) + ")" : string.Empty),
                x.EventType != null ? x.EventType.EventTypeName : string.Empty,
                x.Message})
                });
            }

            return Json(new { success = false, responseText = responseData.Message });
        }

        [HttpPost]
        public async Task<ActionResult> GetFileLogByMonthForMobile(string Id)
        {
            string url = _appSettings.ApiUrl + "/Home/GetFileLogByMonthForMobile";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceid=" + Id);
            var responseData = new ResponseData<Dashboard>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;

                return Json(new
                {
                    success = true,
                    responseText = responseData.Message,
                    Data = responseData.Data.Events
                });
            }

            return Json(new { success = false, responseText = responseData.Message });
        }



        [HttpPost]
        public async Task<ActionResult> AddUUIDBySerialNumber(RequestScanQRCode request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.uuid))
                    return Json(new { success = ResponseStatus.Error, responseText = "uuid must not be null" });
                if (string.IsNullOrEmpty(request.serialnumber))
                    return Json(new { success = ResponseStatus.Error, responseText = "serialnumber must not be null" });
                if (string.IsNullOrEmpty(request.phoneType))
                    return Json(new { success = ResponseStatus.Error, responseText = "phoneType must not be null" });
                if (string.IsNullOrEmpty(request.devicetoken))
                    return Json(new { success = ResponseStatus.Error, responseText = "devicetoken must not be null" });
                if (string.IsNullOrEmpty(request.phonenumber))
                    return Json(new { success = ResponseStatus.Error, responseText = "phonenumber must not be null" });

                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/AddUUIDBySerialNumber";
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, request);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                }
                return Json(new { success = responseData.Status, responseText = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DashBoardForMobile(string uuid)
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/DashBoardForMobile";
                client.BaseAddress = new Uri(url);

                if (string.IsNullOrEmpty(uuid))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "uuid must be required" });
                }

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?uuid=" + uuid);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    if (responseData != null && responseData.Data != null && responseData.Data.ApplianceDTO != null && responseData.Data.ApplianceDTO.Count > 0)
                    {
                        return Json(new { success = responseData.Status, responseText = responseData.Message, data = responseData.Data.ApplianceDTO, iotcloud = responseData.Data.IsIOTConnected });
                    }
                    else
                    {
                        return Json(new { success = responseData.Status, responseText = ResponseMessage.HaveNoAppliance, data = "" });
                    }
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }

        }

        [HttpPost]
        public async Task<ActionResult> IsPhoneNumberExist(string uuid)
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/IsPhoneNumberExist";
                client.BaseAddress = new Uri(url);

                if (string.IsNullOrEmpty(uuid))
                {
                    return Json(new { success = ResponseStatus.Success, responseText = "uuid must be required" });
                }

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?uuid=" + uuid);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    return Json(new { success = responseData.Status, responseText = responseData.Message });
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }

        }

        [HttpPost]
        public async Task<ActionResult> AddPhoneNumber(string uuid, string PhoneNumber, string deviceToken)
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/AddPhoneNumber";
                client.BaseAddress = new Uri(url);

                if (string.IsNullOrEmpty(uuid))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "uuid must be required" });
                }

                if (string.IsNullOrEmpty(PhoneNumber))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "PhoneNumber must be required" });
                }

                if (string.IsNullOrEmpty(deviceToken))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "deviceToken must be required" });
                }

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?uuid=" + uuid + "&phoneNumber=" + PhoneNumber + "&deviceToken=" + deviceToken);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    return Json(new { success = responseData.Status, responseText = responseData.Message });
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateGeoFenceTrip(string uuid, string lat, string lon)
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/UpdateGeoFenceTrip";
                client.BaseAddress = new Uri(url);

                if (string.IsNullOrEmpty(uuid))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "uuid must be required" });
                }

                if (string.IsNullOrEmpty(lat))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "lat must be required" });
                }
                if (string.IsNullOrEmpty(lon))
                {
                    return Json(new { success = ResponseStatus.Error, responseText = "lon must be required" });
                }

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?uuid=" + uuid + "&lat=" + lat + "&lon=" + lon);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    return Json(new { success = responseData.Status, responseText = responseData.Message });
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ChangeApplianceForMobile(RequestFromMobile request)
        {
            try
            {
                string url = _appSettings.ApiUrl + "/Home/UpdateApplianceForMobile";
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceid=" + request.applianceId + "&uuId=" + request.uuid + "&power=" + (request.power ? "on" : "off"));
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    return Json(new { success = responseData.Status, responseText = responseData.Message });
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Purchase(string planId, string nameOnCard, string cc_number, string expireMonth, string expireYear, string ccvCode, string coupon)
        {
            PurchaseInformation info = new PurchaseInformation()
            {
                CCVCode = ccvCode,
                CC_number = cc_number,
                CurrenApplianceId = current_ApplianceId,
                CurrenUserId = current_UserID,
                ExpireMonth = expireMonth,
                ExpireYear = expireYear,
                NameOnCard = nameOnCard,
                PlanId = planId,
                Coupon = coupon
            };

            string url = _appSettings.ApiUrl + "/Home/Purchase";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, info);
            var responseData = new ResponseData<StripePlan>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<StripePlan>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    string message = string.Format("Thank you for subscribing to the AirGap service. " +
                    "Your trial period begins immediately. After {0} days, your credit card will be charged {1:C} per month. " +
                    "You can cancel at any time.", responseData.Data.TrialPeriodDays, Convert.ToDecimal(responseData.Data.Amount / 100m));
                    return Json(new { success = responseData.Status, responseText = message });
                }
                return Json(new { success = ResponseStatus.Success, responseText = responseData.Message });
            }
            return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
        }

        [HttpPost]
        public async Task<ActionResult> CheckCoupon(string coupon)
        {
            string url = _appSettings.ApiUrl + "/Home/CheckCoupon";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?coupon=" + coupon);
            var responseData = new ResponseData<Dashboard>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                if (responseData.Data != null && responseData.Data.Plans != null && responseData.Data.Plans.Count() > 0)
                {
                    return Json(new { success = responseData.Status, responseText = responseData.Message, plan = responseData.Data.Plans, percentOff = responseData.Data.PercentOff, amountOff = responseData.Data.AmountOff });
                }
                return Json(new { success = responseData.Status, responseText = responseData.Message });
            }
            return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
        }


        [HttpPost]
        public async Task<ActionResult> CancelSubscription()
        {
            try
            {
                var response = new ResponseData<Dashboard>();
                string url = _appSettings.ApiUrl + "/Home/CancelSubscription";
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceId=" + current_ApplianceId + "&accountId=" + current_UserID);
                var responseData = new ResponseData<Dashboard>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Dashboard>>().Result;
                    return Json(new { success = responseData.Status, responseText = responseData.Message });
                }
                return Json(new { success = ResponseStatus.Error, responseText = ResponseMessage.Error });
            }
            catch (Exception ex)
            {
                return Json(new { success = ResponseStatus.Error, responseText = ex.Message });
            }

        }
    }

    public class RequestFromMobile
    {
        public string applianceId { get; set; }
        public string uuid { get; set; }
        public bool power { get; set; }
    }

}
