    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Airgap.Constant;
using Microsoft.Extensions.Options;
using Airgap.Entity.Entities;
using Airgap.Service.Helper;
using Airgap.WebUi.Models;
using Microsoft.AspNetCore.Hosting;

namespace Airgap.WebUi.Controllers
{
    public class LoginController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        private IHelperService _helperService;
        private IHostingEnvironment _env;
        public LoginController(IHostingEnvironment env, IHelperService helperService, IOptions<AppSetting> appSettings)
        {
            _env = env;
            _appSettings = appSettings.Value;
            this._helperService = helperService;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Signin()
        {
            if (current_UserID > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Signin(string email, string password)
        {
            string url = _appSettings.ApiUrl + "/Login/Signin";
            client.BaseAddress = new Uri(url);
            var account = new Account() { Email = email, Password = password };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;

                if (responseData.Message == ResponseMessage.EmailNotValid)
                {
                    ViewBag.EmailMessage = responseData.Message;
                    return View();
                }

                if (responseData.Data != null)
                {
                    HttpContext.Session.SetString("UserId", $"{responseData.Data.Id}");
                    if(responseData.Data.AccountAppliances != null && responseData.Data.AccountAppliances.Count() > 0)
                    {
                        HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.AccountAppliances.FirstOrDefault().ApplianceId}");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Setup", new { });
                    }
                    return RedirectToAction("Index", "Home", new { });
                }
                else
                {
                    ViewBag.EmailMessage = "error occurred during process";
                    return View();
                }
            }
            return RedirectToAction("Signin", "Login", new { });

        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Signup(SignupViewModels model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string url = _appSettings.ApiUrl + "/Login/Signup";
            client.BaseAddress = new Uri(url);
            var account = new Account()
            {
                Email = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName,
                IsAdmin = true
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData.Data != null)
                {
                    string linkResetPassword = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/CreatePassword";
                    var parametersToAdd = new System.Collections.Generic.Dictionary<string, string> {{ "token", responseData.Data.AccountTokens.FirstOrDefault().Token }};
                    var link = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(linkResetPassword, parametersToAdd);
                    var path = System.IO.Path.Combine(_env.WebRootPath, "HtmlTemplate.html");
                    _helperService.ExecuteSendMail(model.Email, Configuration.MailSubjectForSignUp, path, link, responseData.Data, Configuration.ContentForSignUp);
                    
                    //return RedirectToAction("Signin", "Login", new { });
                    ViewBag.Message = responseData.Message;
                    return View();
                }
                else
                {
                    ViewBag.MessageError = responseData.Message;
                    return View(model); 
                }
            }
            return RedirectToAction("Signup", "Login", new { });
        }
        [HttpGet]
        public async Task<ActionResult> CreatePassword(string token)
        {
            string url = _appSettings.ApiUrl + "/Login/CheckAccountToken";
            client.BaseAddress = new Uri(url);

            var accountToken = new AccountToken()
            {
                Token = token
            };

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, accountToken);

            var responseData = new ResponseData<AccountToken>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<AccountToken>>().Result;
                if(responseData.Status == ResponseStatus.Error.ToString())
                {
                    return View("InvalidToken");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePassword(string token, string password, string confirmPassword)
        {
            string url = _appSettings.ApiUrl + "/Login/UpdatePassword";
            client.BaseAddress = new Uri(url);
            var accountToken = new AccountToken() {
                Token = token,
            };

            var account = new Account()
            {
                Password = password,
            };
            account.AccountTokens.Add(accountToken);

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;

                return Json(new { success = true, responseText = responseData.Message, url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/Signin" });
            }
            return Json(new { success = false, responseText = responseData.Message });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            string url = _appSettings.ApiUrl + "/Login/ForgotPassword";
            client.BaseAddress = new Uri(url);
            var account = new Account()
            {
                Email = email,
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData.Data != null && responseData.Data.AccountTokens != null && responseData.Data.AccountTokens.Count() > 0)
                {
                    string linkResetPassword = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/CreatePassword";
                    var parametersToAdd = new System.Collections.Generic.Dictionary<string, string> { { "token", responseData.Data.AccountTokens.FirstOrDefault().Token } };
                    var link = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(linkResetPassword, parametersToAdd);
                    var path = System.IO.Path.Combine(_env.WebRootPath, "HtmlTemplate.html");
                    _helperService.ExecuteSendMail(responseData.Data.Email, Configuration.MailSubjectForResetPassword, path, link, responseData.Data, Configuration.ContentForResetPassword);
                    return Json(new { success = true, responseText = responseData.Message, url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/Signin" });
                }
            }
            return Json(new { success = false, responseText = responseData.Message });

        }

        [HttpPost]
        public async Task<ActionResult> CreateUserWithUserFacebook(string firstname, string lastname, string email, string appId)
        {
            string url = _appSettings.ApiUrl + "/Login/CreateUserWithUserFacebook";
            client.BaseAddress = new Uri(url);
            var account = new Account()
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                FaceBookId = appId
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData.Data != null)
                {
                    HttpContext.Session.SetString("UserId", $"{responseData.Data.Id}");

                    if (string.IsNullOrEmpty(responseData.Data.Email))
                    {
                        return Json(new { success = true, responseText = responseData.Message, url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/UpdateInformation?firstname=" + firstname + "&lastname=" +lastname });
                    }

                    if (responseData.Data.AccountAppliances != null && responseData.Data.AccountAppliances.Count() > 0)
                    {
                        HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.AccountAppliances.FirstOrDefault().ApplianceId}");
                    }
                    else
                    {
                        return Json(new { success = true, responseText = responseData.Message, url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Setup/Index" });
                    }
                    return Json(new { success = true, responseText = responseData.Message, url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Home/Index" });
                }
                else
                {
                    ViewBag.EmailMessage = "error occurred during process";
                    return View();
                }
            }
            return Json(new { success = false, responseText = responseData.Message });
        }

        public ActionResult UpdateInformation(string firstname, string lastname)
        {
            SignupViewModels model = new SignupViewModels();
            model.FirstName = firstname;
            model.LastName = lastname;
            return View("UpdateInformation",model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateInformation(SignupViewModels model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string url = _appSettings.ApiUrl + "/Login/UpdateInformation";
            client.BaseAddress = new Uri(url);
            var account = new Account()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Id = current_UserID
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(url, account);

            var responseData = new ResponseData<Account>();
            if (responseMessage.IsSuccessStatusCode)
            {
                responseData = responseMessage.Content.ReadAsAsync<ResponseData<Account>>().Result;
                if (responseData.Data != null)
                {
                    if (responseData.Data.AccountAppliances != null && responseData.Data.AccountAppliances.Count() > 0)
                    {
                        HttpContext.Session.SetString("ApplianceId", $"{responseData.Data.AccountAppliances.FirstOrDefault().ApplianceId}");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Setup");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.EmailMessage = "error occurred during process";
                    return View();
                }
            }

            return View();
        }

        public ActionResult LogOff()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("ApplianceId");
            return RedirectToAction("Signin", "Login");
        }
    }
}