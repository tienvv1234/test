using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Constant;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Airgap.Data.ApiEntities;
using Airgap.Data.DTOEntities;
using Airgap.WebUi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace Airgap.WebUi.Controllers
{
    public class AirController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        List<SelectListItem> listStates;
        public AirController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<ActionResult> Index()
        {
            AirViewModels model = new AirViewModels();
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

                ViewBag.ActiveMenu = "air";
                string url = _appSettings.ApiUrl + "/Air/Index";
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?accountid=" + current_UserID + "&applianceid=" + current_ApplianceId);

                var responseData = new ResponseData<AirGapSetting>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<AirGapSetting>>().Result;
                    
                    if (responseData.Data != null)
                    {
                        if (responseData.Data.ListApplianceDTO != null && responseData.Data.ListApplianceDTO.Count > 0)
                        {
                            foreach (var item in responseData.Data.ListApplianceDTO)
                            {
                                item.DeviceName += "(" + item.SerialNumber.Substring(item.SerialNumber.Length - 4) + ")";
                                if (item.Id == applianceId)
                                {
                                    model.SelectedAppliance = item;
                                    model.StreetName = item.Street1;
                                    model.StreetName2 = item.Street2;
                                    model.State = item.StateId.ToString();
                                    model.City = item.City;
                                    model.Zipcode = item.ZipCode;
                                }
                            }
                            model.lAppliance = responseData.Data.ListApplianceDTO;
                        }

                        if (responseData.Data.TimerScheduleDTO != null && responseData.Data.TimerScheduleDTO.Count > 0)
                        {
                            Dictionary<long, string[]> lTimer = new Dictionary<long, string[]>();
                            foreach (var item in responseData.Data.TimerScheduleDTO)
                            {
                                
                                if (!string.IsNullOrEmpty(item.ActiveValues))
                                {
                                    if(item.ActiveValues.IndexOf(',') != -1)
                                    {
                                        lTimer.Add(item.TimerTypeId.Value , item.ActiveValues.Split(','));
                                    }
                                    else
                                    {
                                        lTimer.Add(item.TimerTypeId.Value, new string[] { item.ActiveValues});
                                    }
                                }
                            }
                            model.lTimerSchedule = lTimer;
                        }

                        if (responseData.Data.States != null && responseData.Data.States.Count() > 0)
                        {
                            var listStates = new List<SelectListItem>();
                            foreach (var item in responseData.Data.States)
                            {
                                listStates.Add(new SelectListItem
                                {
                                    Text = item.Name,
                                    Value = item.Id.ToString(),
                                    Selected = item.Id == model.SelectedAppliance.StateId ? true : false
                                });
                            }
                            model.States = listStates;
                        }

                        if (responseData.Data.ListAccountDTO != null && responseData.Data.ListAccountDTO.Count() > 0)
                        {
                            model.lAccountDTO = responseData.Data.ListAccountDTO;
                        }
                    }

                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(AirViewModels model)
        {
            GetStates().Wait();
            model.States = listStates;
            string url = string.Empty;
            long applianceId = 0;
            if (current_ApplianceId > 0)
            {
                applianceId = current_ApplianceId;
            }
            if (model.UseAddressFromTelit == "UseTelit")
            {
                url = _appSettings.ApiUrl + "/Air/UseAddressOfTelit";
                client = new HttpClient();
                client.BaseAddress = new Uri(url);

            }
            else
            {
                url = _appSettings.ApiUrl + "/Air/CompareAddressFromTelit";
                client = new HttpClient();
                client.BaseAddress = new Uri(url);
            }
            
            ApplianceDTO applianceDTO = new ApplianceDTO()
            {
                AccountId = current_UserID,
                Street1 = model.StreetName,
                Street2 = model.StreetName2,
                City = model.City,
                ZipCode = model.Zipcode,
                StateId = Convert.ToInt64(model.State),
                Id = current_ApplianceId
            };
            
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, applianceDTO);

            var responseData = new ResponseData<AirGapSetting>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AirGapSetting>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    if (responseData.Data.ListApplianceDTO != null && responseData.Data.ListApplianceDTO.Count > 0)
                    {
                        foreach (var item in responseData.Data.ListApplianceDTO)
                        {
                            item.DeviceName += "(" + item.SerialNumber.Substring(item.SerialNumber.Length - 4) + ")";
                            if (item.Id == applianceId)
                            {
                                model.SelectedAppliance = item;
                                model.StreetName = item.Street1;
                                model.StreetName2 = item.Street2;
                                model.State = item.StateId.ToString();
                                model.City = item.City;
                                model.Zipcode = item.ZipCode;
                            }
                        }
                        model.lAppliance = responseData.Data.ListApplianceDTO;
                    }

                    if (responseData.Data.TimerScheduleDTO != null && responseData.Data.TimerScheduleDTO.Count > 0)
                    {
                        Dictionary<long, string[]> lTimer = new Dictionary<long, string[]>();
                        foreach (var item in responseData.Data.TimerScheduleDTO)
                        {

                            if (!string.IsNullOrEmpty(item.ActiveValues))
                            {
                                if (item.ActiveValues.IndexOf(',') != -1)
                                {
                                    lTimer.Add(item.TimerTypeId.Value, item.ActiveValues.Split(','));
                                }
                                else
                                {
                                    lTimer.Add(item.TimerTypeId.Value, new string[] { item.ActiveValues });
                                }
                            }
                        }
                        model.lTimerSchedule = lTimer;
                    }

                    if (responseData.Data.States != null && responseData.Data.States.Count() > 0)
                    {
                        var listStates = new List<SelectListItem>();
                        foreach (var item in responseData.Data.States)
                        {
                            listStates.Add(new SelectListItem
                            {
                                Text = item.Name,
                                Value = item.Id.ToString(),
                                Selected = item.Id == model.SelectedAppliance.StateId ? true : false
                            });
                        }
                        model.States = listStates;
                    }

                    if (responseData.Data.ListAccountDTO != null && responseData.Data.ListAccountDTO.Count() > 0)
                    {
                        model.lAccountDTO = responseData.Data.ListAccountDTO;
                    }

                    if (responseData.Data.IsMatching != null && responseData.Data.IsMatching.HasValue)
                    {

                        var isMatching = responseData.Data.IsMatching.Value;
                        if (isMatching)
                        {
                            return RedirectToAction("Index", "Air", new { });
                        }
                        else
                        {
                            ViewBag.ConfirmAddress = true;
                        }
                    }
                    else
                    {
                        if (responseData.Data != null && responseData.Status == ResponseStatus.Success.ToString())
                        {
                            return RedirectToAction("Index", "Air", new { });
                        }
                        else
                        {
                            ViewBag.MessageError = responseData.Message;
                        }
                    }
                }
                else
                {
                    ViewBag.MessageError = responseData.Message;
                }
            }

            return View(model);
        }

        public async Task GetStates()
        {

            string url = _appSettings.ApiUrl + "/Setting/Index";
            client.BaseAddress = new Uri(url);
            HttpResponseMessage responseMessage = await client.GetAsync(url + "?id=" + current_UserID);
            var responseData = new ResponseData<Settings>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Settings>>().Result;
                if (responseData != null && responseData.Data != null && responseData.Data.States != null && responseData.Data.States.Count > 0)
                {
                    listStates = new List<SelectListItem>();
                    foreach (var item in responseData.Data.States)
                    {
                        listStates.Add(new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString()
                        });
                    }
                }
            }
        }

        public async Task<ActionResult> DeleteSerialNumber(string id)
        {
            ViewBag.ActiveMenu = "air";
            string url = _appSettings.ApiUrl + "/Air/deleteSerialNumber";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceId=" + id + "&accountId=" + current_UserID);

            var responseData = new ResponseData<AirGapSetting>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AirGapSetting>>().Result;
                if (responseData.Status == ResponseStatus.Success.ToString())
                {
                    if(responseData.Data != null && responseData.Data.ListApplianceDTO != null && responseData.Data.ListApplianceDTO.Count() > 0)
                    {

                        HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.ListApplianceDTO.FirstOrDefault().Id}");
                    }
                    else
                    {
                        HttpContext.Session.Remove("ApplianceId");
                    }
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        public ActionResult ChangeAppliance(string applianceId)
        {
            if (applianceId != HttpContext.Session.GetString("ApplianceId"))
            {
                HttpContext.Session.SetString("ApplianceId", $"{applianceId}");
            }

            return RedirectToAction("Index", "Air", new { });
        }

        public async Task<ActionResult> UpdateNotificationPreference(string hdValue)
        {
            ViewBag.ActiveMenu = "air";
            string url = _appSettings.ApiUrl + "/Air/UpdateNotificationPreference";
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?value=" + hdValue + "&applianceId=" + current_ApplianceId);


            var responseData = new ResponseData<AirGapSetting>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AirGapSetting>>().Result;
            }
            return RedirectToAction("Index", "Air", new { });
        }
    }
}