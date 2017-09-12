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
using Airgap.WebUi.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Airgap.Data.DTOEntities;
using Microsoft.AspNetCore.Http;

namespace Airgap.WebUi.Controllers
{
    public class SetupController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        List<SelectListItem> listStates;
        public SetupController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Index()
        {
            GetStates().Wait();
            SetUpViewModels setup = null;
            if(current_ApplianceId > 0)
            {
                ViewBag.BackButton = true;
            }


            if (current_UserID <= 0)
            {
                return RedirectToAction("Signin", "Login");
            }
            else
            {
                setup = new SetUpViewModels()
                {
                    States = listStates
                };
                
            }
            return View(setup);
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

        [HttpPost]
        public async Task<ActionResult> Index(SetUpViewModels model)
        {
            if (current_ApplianceId > 0)
            {
                ViewBag.BackButton = true;
            }
            string url = string.Empty;
            GetStates().Wait();
            model.States = listStates;
            if (model.UseAddressFromTelit == "UseTelit")
            {
                url = _appSettings.ApiUrl + "/Setting/UseAddressOfTelit";
                client = new HttpClient();
                client.BaseAddress = new Uri(url);
                
            }
            else
            {
                url = _appSettings.ApiUrl + "/Setting/SetupThink";
                client = new HttpClient();
                client.BaseAddress = new Uri(url);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!ValidateSerinumber(model.SerialNumber.Trim()))
            {
                ViewBag.MessageError = "SerialNumber is invalid";
                return View(model);
            }
            

            ApplianceDTO applianceDTO = new ApplianceDTO()
            {
                AccountId = current_UserID,
                SerialNumber = model.SerialNumber.Trim(),
                Street1 = model.StreetName.Trim(),
                Street2 = model.StreetName2 != null ? model.StreetName2.Trim() : string.Empty,
                City = model.City.Trim(),
                ZipCode = model.Zipcode.Trim(),
                StateId = Convert.ToInt64(model.State)
            };

            

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, applianceDTO);

            var responseData = new ResponseData<ApplianceDTO>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<ApplianceDTO>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    if(responseData.Data.HasSerialNumber != null && responseData.Data.HasSerialNumber.HasValue && responseData.Data.IsMatching != null && responseData.Data.IsMatching.HasValue)
                    {

                        var hasSerialNumber = responseData.Data.HasSerialNumber.Value;
                        var isMatching = responseData.Data.IsMatching.Value;
                        if (hasSerialNumber)
                        {
                            if (isMatching)
                            {
                                HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.Id}");
                                return RedirectToAction("Index", "Home", new { });
                            }
                            else
                            {
                                ViewBag.ConfirmAddress = true;
                            }
                        }
                        else
                        {
                            ViewBag.MessageError = responseData.Message;
                        }
                    }
                    else
                    {
                        if (responseData.Data != null && responseData.Status == ResponseStatus.Success.ToString())
                        {
                            HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.Id}");
                            return RedirectToAction("Index", "Home", new { });
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


        [HttpPost]
        public async Task<ActionResult> UseAddressOfTelit(SetUpViewModels model)
        {

            ApplianceDTO applianceDTO = new ApplianceDTO()
            {
                AccountId = current_UserID,
                SerialNumber = model.SerialNumber,
                Street1 = model.StreetName,
                Street2 = model.StreetName2,
                City = model.City,
                ZipCode = model.Zipcode,
                StateId = Convert.ToInt64(model.State)
            };

            string url = _appSettings.ApiUrl + "/Setting/UseAddressOfTelit";
            client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, applianceDTO);

            var responseData = new ResponseData<ApplianceDTO>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<ApplianceDTO>>().Result;
                if(responseData.Status == ResponseMessage.Success && responseData.Data != null)
                {
                    HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.Id}");
                    return RedirectToAction("Index", "Home", new { });
                }
                else
                {
                    ViewBag.MessageError = responseData.Message;
                }
            }
            
            return View(model);
        }

        private bool ValidateSerinumber(string serialNumber)
        {
            try
            {
                 int hour = -1;
                int second = -1;
                int u = -1;
                DateTime date = new DateTime();
                if (serialNumber.Length == 16)
                {
                    string dd = serialNumber.Substring(0, 2);
                    string mm = serialNumber.Substring(2, 2);
                    string yy = serialNumber.Substring(4, 2);
                    string hh = serialNumber.Substring(6, 2);
                    string ss = serialNumber.Substring(8, 2);
                    string uuuu = serialNumber.Substring(10, 4);

                    if (!DateTime.TryParse(mm + "/" + dd + "/20" + yy, out date))
                    {
                        return false;
                    }

                    if (!int.TryParse(hh, out hour) && hour < 0 && hour >= 24)
                    {
                        return false;
                    }

                    if (!int.TryParse(ss, out second) && second < 0 && second >= 60)
                    {
                        return false;
                    }

                    if (!int.TryParse(uuuu, out u) && u < 0 && u > 9999)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
    }
}