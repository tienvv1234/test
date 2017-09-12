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
using Airgap.Data.ApiEntities;

namespace Airgap.WebUi.Controllers
{
    public class SettingController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        public SettingController(IOptions<AppSetting> appSettings)
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
                return RedirectToAction("Setup", "Index");
            }
            else
            {
                long applianceId = 0;
                if(current_ApplianceId > 0)
                {
                    applianceId = current_ApplianceId;
                }

                ViewBag.ActiveMenu = "setting";
                string url = _appSettings.ApiUrl + "/Setting/Index";
                client.BaseAddress = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(url + "?id=" + current_UserID);

                var responseData = new ResponseData<Settings>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Settings>>().Result;
                    ViewBag.States = responseData != null && responseData.Data != null && responseData.Data.States != null && responseData.Data.States.Count > 0 ? responseData.Data.States : null;
                    ViewBag.Appliances = responseData != null && responseData.Data != null && responseData.Data.Appliances != null && responseData.Data.Appliances.Count > 0 ? responseData.Data.Appliances : null;
                }

                return View();
            }

        }

    }
}