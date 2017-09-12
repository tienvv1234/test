using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Stripe;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Airgap.Constant;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Reflection;

namespace Airgap.WebUi.Controllers
{
    public class StripeCallBackController : BaseController
    {
        private readonly AppSetting _appSettings;
        private HttpClient client;
        public StripeCallBackController(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(CancellationToken ct)
        {
            try
            {
                var response = new ResponseData<bool>();
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                string url = _appSettings.ApiUrl + "/StripeCallBack/UpdateAppliance";
                client.BaseAddress = new Uri(url);
                var stripeEvent = StripeEventUtility.ParseEvent(json);
                StripeSubscription stripeCharge = null;
                //StripeCharge chargeFailed = null;
                //StripeCharge chargeSucceeded = null;
                StripeSubscription customerSubscriptionUpdate = null;
                StripeSubscription customerSubscriptionDelete = null;
                StripeCustomer stripeCustomer = null;
                switch (stripeEvent.Type)
                {
                    case StripeEvents.CustomerSubscriptionCreated:  // all of the types available are listed in StripeEvents
                        stripeCharge = Stripe.Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
                        break;
                    case StripeEvents.CustomerSubscriptionUpdated:
                        customerSubscriptionUpdate = Stripe.Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
                        break;
                    case StripeEvents.CustomerSubscriptionDeleted:
                        customerSubscriptionDelete = Stripe.Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
                        break;
                    case StripeEvents.CustomerDeleted:
                        stripeCustomer = Stripe.Mapper<StripeCustomer>.MapFromJson(stripeEvent.Data.Object.ToString());
                        break;


                }

                HttpResponseMessage responseMessage = null;

                if(stripeCustomer != null)
                {
                    string urlDeleteCustomer = _appSettings.ApiUrl + "/StripeCallBack/DeleteCustomer";
                    client = new HttpClient();
                    client.BaseAddress = new Uri(urlDeleteCustomer);
                    responseMessage = await client.GetAsync(url + "?customerIdOfStripe=" + stripeCustomer.Id);
                }


                if(stripeCharge != null)
                {
                    
                    if (stripeCharge.Status == StripeSubscriptionStatuses.Active || stripeCharge.Status == StripeSubscriptionStatuses.Trialing)
                    {
                        responseMessage = await client.GetAsync(url + "?subscriptionId=" + stripeCharge.Id + "&update=" + Configuration.Enable);
                    }
                    else
                    {
                        responseMessage = await client.GetAsync(url + "?subscriptionId=" + stripeCharge.Id + "&update=" + Configuration.Disable);
                    }
                }

                if (customerSubscriptionDelete != null)
                {

                    responseMessage = await client.GetAsync(url + "?subscriptionId=" + customerSubscriptionDelete.Id + "&update=" + Configuration.Disable);
                }

                if (customerSubscriptionUpdate != null)
                {

                    if (customerSubscriptionUpdate.Status == StripeSubscriptionStatuses.Active || customerSubscriptionUpdate.Status == StripeSubscriptionStatuses.Trialing)
                    {
                        responseMessage = await client.GetAsync(url + "?subscriptionId=" + customerSubscriptionUpdate.Id + "&update=" + Configuration.Enable);
                    }
                    else
                    {
                        responseMessage = await client.GetAsync(url + "?subscriptionId=" + customerSubscriptionUpdate.Id + "&update=" + Configuration.Disable);
                    }
                }

                var responseData = new ResponseData<bool>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseData = responseMessage.Content.ReadAsAsync<ResponseData<bool>>().Result;
                }
                return Json(new { success = true, message = responseData.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}