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

namespace Airgap.WebUi.Controllers
{
    public class TelitGatewayController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        public TelitGatewayController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ActionResult> Index()
        {
            //if (current_UserID <= 0)
            //{
            //    return RedirectToAction("Signin", "Login");
            //}
            //else
            //{
            //    ViewBag.ActiveMenu = "dashboard";
            //    string url = _appSettings.ApiUrl + "/Home/Index";
            //    client.BaseAddress = new Uri(url);
            //    string parameter = "";
            //    HttpResponseMessage responseMessage = await client.GetAsync(url + parameter);

            //    var responseData = new ResponseData<TimerType>();
            //    if (responseMessage.IsSuccessStatusCode)
            //    {
            //        responseData = responseMessage.Content.ReadAsAsync<ResponseData<TimerType>>().Result;
            //    }

              return View();
            //}
            
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusFromTelit([FromBody] RequestTelit request)
        {
            try
            {
                string url = _appSettings.ApiUrl + "/TelitGateway/UpdateStatusFromTelit";
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, request);

                var responseData = new ResponseData<RequestTelit>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<RequestTelit>>().Result;
                }
                return Json(new { success = true, responseText = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
