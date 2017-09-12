using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Constant;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Airgap.Entity.Entities;
using Airgap.Data.DTOEntities;
using Airgap.WebUi.Models;
using Airgap.Data.ApiEntities;
using Microsoft.AspNetCore.Http;

namespace Airgap.WebUi.Controllers
{
    public class UserController : BaseController
    {

        private readonly AppSetting _appSettings;
        private HttpClient client;
        public UserController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> Index()
        {
            UserViewModels model = new UserViewModels();
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
                ViewBag.ActiveMenu = "user";   
                string url = _appSettings.ApiUrl + "/User/Index";
                client = new HttpClient();
                client.BaseAddress = new Uri(url);

                HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceId=" + current_ApplianceId + "&AccountId=" + current_UserID);
                var responseData = new ResponseData<User>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<User>>().Result;
                    if(responseData != null && responseData.Data != null)
                    {
                        ApplianceDTO selectedAppliance = null;;
                        if (responseData.Data.lAppliance != null && responseData.Data.lAppliance.Count > 0)
                        {
                            foreach (var item in responseData.Data.lAppliance)
                            {
                                item.DeviceName += "(" + item.SerialNumber.Substring(item.SerialNumber.Length - 4) + ")";
                                if (item.Id == current_ApplianceId)
                                {
                                    selectedAppliance = item;
                                }
                            }
                        }
                        model.lAccount = responseData.Data.lAccountDTO;
                        model.lAppliance = responseData.Data.lAppliance;
                        model.SelectedAppliance = selectedAppliance;
                    }
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserViewModels model)
        {
            string url = _appSettings.ApiUrl + "/User/UpdateUser";
            client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?applianceId=" + current_ApplianceId + "&currenctUserId=" + current_UserID + "&accountId=" + model.hdAccountId + "&firstName=" + model.FirstName + "&lastName=" + model.LastName + "&isVerified=" + model.Verify);
            var responseData = new ResponseData<AccountDTO>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AccountDTO>>().Result;
                if (responseData != null && responseData.DataList != null && responseData.DataList.Count > 0)
                {
                    model.lAccount = responseData.DataList;
                }
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string accountId)
        {
            string url = _appSettings.ApiUrl + "/User/DeleteUser";
            client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpResponseMessage responseMessage = await client.GetAsync(url + "?accountId=" + accountId + "&applianceId=" + current_ApplianceId);
            var responseData = new ResponseData<AccountDTO>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AccountDTO>>().Result;
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult ChangeAppliance(string applianceId)
        {
            if (applianceId != HttpContext.Session.GetString("ApplianceId"))
            {
                HttpContext.Session.SetString("ApplianceId", $"{applianceId}");
            }

            return RedirectToAction("Index", "User", new { });
        }
    }
}