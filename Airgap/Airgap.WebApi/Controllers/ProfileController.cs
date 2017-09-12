using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airgap.Entity;
using Airgap.Entity.Entities;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using Airgap.Service;
using Airgap.Service.Helper;

namespace Airgap.WebApi.Controllers
{
    public class ProfileController : BaseController
    {
        private IAccountService _accountService;
        private IHelperService _helperService;
        private IPasswordHistoryService _passwordHistoryService;
        public ProfileController(IPasswordHistoryService passwordHistoryService, IAccountService accountService, IHelperService helperService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._passwordHistoryService = passwordHistoryService;
            this._accountService = accountService;
            this._helperService = helperService;
        }

        [HttpGet]
        public ResponseData<Account> GetProfile(string accountid)
        {
            try
            {
                var acc = _accountService.GetAccountById(Convert.ToInt64(accountid));
                
                var response = new ResponseData<Account>();

                response.Data = acc;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Account>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public ResponseData<Account> UpdateProfile([FromBody]Account account)
        {
            try
            {
                var acc = _accountService.GetAccountById(Convert.ToInt64(account.Id));
                acc.FirstName = account.FirstName;
                acc.LastName = account.LastName;
                _accountService.Update(acc);
                var response = new ResponseData<Account>();

                response.Data = acc;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Account>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public ResponseData<Account> ChangePassword([FromBody]Account account)
        {
            try
            {
                var response = new ResponseData<Account>();
                var acc = _accountService.GetAccountById(Convert.ToInt64(account.Id));
                if (acc.Password == (account.LastPasswords == null ? null : _helperService.Hashing(account.LastPasswords)))
                {
                    acc.Password = _helperService.Hashing(account.Password);
                }
                else
                {
                    response.Message = Configuration.PasswordNotMatch;
                    response.Status = ResponseStatus.Success.ToString();
                    return response;
                }
                _accountService.Update(acc);

                response.Data = acc;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<Account>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }
    }
}