using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Service;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using Airgap.WebUi.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using Airgap.Entity.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Airgap.WebUi.Controllers
{
    public class ProfileController : BaseController
    {

        private readonly AppSetting _appSettings;
        private HttpClient client;
        public ProfileController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<IActionResult> Index(bool? success)
        {
            if(success != null && success.HasValue && success.Value)
            {
                ViewBag.ChangePassSuccessfull = true;
            }

            if(success != null && success.HasValue && !success.Value)
            {
                ViewBag.PasswordIsNotCorrect = true;
            }
            ViewBag.ActiveMenu = "profile";
            ProfileViewModels profile = null;

            if (current_UserID <= 0)
            {
                return RedirectToAction("Signin", "Login");
            }else if(current_ApplianceId <= 0)
            {
                return RedirectToAction("Index", "Setup");
            }
            else
            {

                string url = _appSettings.ApiUrl + "/Profile/GetProfile";
                client.BaseAddress = new Uri(url);
                HttpResponseMessage responseMessage = await client.GetAsync(url + "?accountid=" + current_UserID);
                var responseData = new ResponseData<Account>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                    if (responseData != null && responseData.Data != null) {
                        profile = new ProfileViewModels
                        {
                            EmailAddress = responseData.Data.Email,
                            FirstName = responseData.Data.FirstName,
                            LastName = responseData.Data.LastName,
                            PhoneNumber = responseData.Data.PhoneNumber
                        };
                    }
                }
            }

            return View(profile);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ProfileViewModels model)
        {
            Account acc = new Account()
            {
                Email = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Id = current_UserID
            };

            string url = _appSettings.ApiUrl + "/Profile/UpdateProfile";
            client = new HttpClient();

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, acc);
            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    model.EmailAddress = responseData.Data.Email;
                    model.FirstName = responseData.Data.FirstName;
                    model.LastName = responseData.Data.LastName;
                    model.PhoneNumber = responseData.Data.PhoneNumber;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ProfileViewModels model)
        {
            Account acc = new Account()
            {
                LastPasswords = model.ExistingPassword,
                Password = model.NewPassword,
                Id = current_UserID
            };

            string url = _appSettings.ApiUrl + "/Profile/ChangePassword";
            client = new HttpClient();

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, acc);
            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData != null && responseData.Data != null)
                {
                    return RedirectToAction("Index", "Profile", new { success = true });
                }
            }
            return RedirectToAction("Index", "Profile", new { success = false });
        }
    }
}