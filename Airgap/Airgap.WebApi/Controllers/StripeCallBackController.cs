using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Constant;
using Airgap.Service;
using Microsoft.Extensions.Options;
using Airgap.Telit;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Airgap.WebApi.Controllers
{
    public class StripeCallBackController : BaseController
    {
        private IAccountApplianceService _accountApplianceService;
        private IApplianceService _applianceService;
        private IAccountService _accountService;

        public StripeCallBackController(IAccountService accountService, IApplianceService applianceService, IAccountApplianceService accountApplianceService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._accountApplianceService = accountApplianceService;
            this._applianceService = applianceService;
            this._accountService = accountService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public async Task<ResponseData<bool>> UpdateAppliance(string subscriptionId, string update)
        {
            try
            {
                var accountAppliance = _accountApplianceService.GetAccountApplianceBySubScription(subscriptionId);
                var appliance = _applianceService.GetApplianceById(accountAppliance.FirstOrDefault().ApplianceId.Value);
                var response = new ResponseData<bool>();
                var thingFind = await TelitApi.ThingFind(appliance.SerialNumber);
                Telit.ThingFind.Params _params = new Telit.ThingFind.Params();
                _params = thingFind.Things.@params;

                var success = await TelitApi.UpdateStatusOfAppliance(_params.iccid, update == Configuration.Disable ? true : false);
                if (success)
                {                    
                    appliance.Status = update == Configuration.Enable ? true : false;
                    _applianceService.Update(appliance);
                    if (!appliance.Status.Value)
                    {
                        accountAppliance.FirstOrDefault().SubscriptionId = null;
                        _accountApplianceService.Update(accountAppliance.FirstOrDefault());
                    }

                    response.Message = ResponseMessage.Success;
                }
                else {
                    response.Message = ResponseMessage.Error;
                }
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<bool>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        public ResponseData<bool> DeleteCustomer(string customerIdOfStripe)
        {
            try
            {
                var account = _accountService.GetAccountByCustomerIdOfStripe(customerIdOfStripe);
                account.CustomerIdStripe = null;
                _accountService.Update(account);
                var response = new ResponseData<bool>();
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<bool>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }
    }
}
