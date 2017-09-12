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
    public class LoginController : BaseController
    {
        private IAccountService _accountService;
        private IAccountTokenService _accountTokenService;
        private IEventService _eventService;
        private IHelperService _helperService;
        private IPasswordHistoryService _passwordHistoryService;
        private IAccountApplianceService _accountApplianceceService;
        
        public LoginController(IAccountApplianceService accountApplianceceService, IAccountService accountService, IAccountTokenService accountTokenService, IEventService eventService, IHelperService helperService, IPasswordHistoryService passwordHistoryService, IOptions<AppSetting> appSettings) : base(appSettings.Value)
        {
            this._accountService = accountService;
            this._accountTokenService = accountTokenService;
            this._eventService = eventService;
            this._helperService = helperService;    
            this._passwordHistoryService = passwordHistoryService;
            this._accountApplianceceService = accountApplianceceService;
        }
        [HttpPost]
        public ResponseData<Account> Signin([FromBody] Account account)
        {
            try
            {
                var response = new ResponseData<Account>();
                account.Password = _helperService.Hashing(account.Password);
                var acc = _accountService.Signin(account.Email, account.Password);
                if (acc == null || (acc != null && !acc.IsAdmin.Value))
                {
                    response.Data = acc;
                    response.Message = Constant.ResponseMessage.EmailNotValid;
                    response.Status = ResponseStatus.Success.ToString();
                    return response;
                }

                var accountAppliance = _accountApplianceceService.GetAccountApplianceByAccountId(acc.Id);
                if(accountAppliance == null && accountAppliance.Count() == 0)
                {
                    response.Data = acc;
                    response.Message = Constant.ResponseMessage.HaveNoAppliance;
                    response.Status = ResponseStatus.Success.ToString();
                    return response;
                }

                var _event = new Event()
                {
                    AccountId = acc.Id,
                    EventTypeId = Constant.EventType.SigninSignoutOfWebPortal,
                    EventDetail = Constant.ResponseMessage.Signin,
                    Timestamp = DateTime.UtcNow,
                    Message = Constant.ResponseMessage.Signin
                };

                _eventService.Insert(_event);

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

        public ResponseData<Account> Signup([FromBody] Account account)
        {
            try
            {
                var response = new ResponseData<Account>();

                var CheckAccountIsExist = _accountService.GetAccountByEmail(account.Email);
                if(CheckAccountIsExist == null)
                {
                    var acc = _accountService.Insert(account);
                    if (acc == null)
                    {
                        response.Data = acc;
                        response.Message = Constant.ResponseMessage.Error;
                        response.Status = ResponseStatus.Error.ToString();
                        return response;
                    }

                    var accToken = new AccountToken()
                    {
                        AccountId = acc.Id,
                        IsSignupToken = true,
                        Timestamp = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString()
                    };

                    acc.AccountTokens.Add(_accountTokenService.Insert(accToken));

                    var _event = new Event()
                    {
                        AccountId = acc.Id,
                        EventTypeId = Constant.EventType.SigninSignoutOfWebPortal,
                        EventDetail = Constant.ResponseMessage.Signin,
                        Timestamp = DateTime.UtcNow,
                        Message = Constant.ResponseMessage.Signin
                    };

                    _eventService.Insert(_event);
                    response.Data = acc;
                    response.Message = Constant.ResponseMessage.Success;
                }
                else
                {
                    response.Message = Constant.ResponseMessage.EmailExist;
                }

                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch(Exception ex)
            {
                var response = new ResponseData<Account>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }


        [HttpPost]
        public ResponseData<Account> ForgotPassword([FromBody]Account account)
        {
            try
            {
                var acc = _accountService.GetAccountByEmail(account.Email);
                var response = new ResponseData<Account>();
                if (acc == null)
                {
                    response.Data = acc;
                    response.Message = Constant.ResponseMessage.EmailIsNotExist;
                    response.Status = ResponseStatus.Success.ToString();
                    return response;
                }

                var token = _accountTokenService.GetAccountTokenByAccountId(acc.Id);
                if(token != null)
                {
                    token.Token = Guid.NewGuid().ToString();
                    token.Timestamp = DateTime.UtcNow;
                    _accountTokenService.Update(token);
                    acc.AccountTokens.Add(token);
                }
                else
                {
                    var accToken = new AccountToken()
                    {
                        AccountId = acc.Id,
                        IsSignupToken = false,
                        Timestamp = DateTime.UtcNow,
                        Token = Guid.NewGuid().ToString()
                    };
                    _accountTokenService.Insert(accToken);
                    acc.AccountTokens.Add(accToken);
                }
                
                var _event = new Event()
                {
                    AccountId = acc.Id,
                    EventTypeId = Constant.EventType.PasswordReset,
                    EventDetail = Constant.ResponseMessage.ForgotPassword,
                    Timestamp = DateTime.UtcNow,
                    Message = Constant.ResponseMessage.ForgotPassword
                };

                _eventService.Insert(_event);

                //string linkResetPassword = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Login/CreatePassword";
                //var parametersToAdd = new System.Collections.Generic.Dictionary<string, string> { { "accountid", acc.Id.ToString() }, { "token", acc.AccountTokens.FirstOrDefault().Token} };
                //var content = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(linkResetPassword, parametersToAdd);

                //_helperService.sendEmail("tien.vuvan@launchdeckpowergate.com", Configuration.MailSubject, content);

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
        public ResponseData<AccountToken> CheckAccountToken([FromBody]AccountToken accountToken)
        {
            try
            {
                var response = new ResponseData<AccountToken>();
                var accToken = _accountTokenService.GetAccountTokenByToken(accountToken.Token);
                if (accToken == null || accountToken.Token != accToken.Token || accToken.Timestamp.Value.AddHours(24) < DateTime.UtcNow)
                {
                    response.Data = accToken;
                    response.Message = Constant.ResponseMessage.Error;
                    response.Status = ResponseStatus.Error.ToString();
                    return response;
                }

                response.Data = accToken;
                response.Message = ResponseMessage.Success;
                response.Status = ResponseStatus.Success.ToString();
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseData<AccountToken>();
                response.Message = ex.Message;
                response.Status = ResponseStatus.Error.ToString();
                return response;
            }
        }

        [HttpPost]
        public ResponseData<Account> UpdatePassword([FromBody]Account account)
        {
            try
            {
                var response = new ResponseData<Account>();
                
                var accToken = _accountTokenService.GetAccountTokenByToken(account.AccountTokens.FirstOrDefault().Token);

                var acc = _accountService.GetAccountById(accToken.AccountId.Value);
                acc.IsVerified = true;
                acc.Password = _helperService.Hashing(account.Password);

                var passwordHistory = new PasswordHistory() {
                    AccountId = accToken.AccountId,
                    CreatedOn = DateTime.UtcNow,
                    Password = acc.Password
                };

                _accountTokenService.Remove(accToken);

                _passwordHistoryService.Insert(passwordHistory);

                _accountService.Update(acc);

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
        public ResponseData<Account> CreateUserWithUserFacebook([FromBody]Account account)
        {
            try
            {
                var response = new ResponseData<Account>();
                account.IsVerified = true;
                account.IsAdmin = true;
                Account acc = null;
                var CheckAccountIsExist = _accountService.GetAccountByFaceBookId(account.FaceBookId);
                if(CheckAccountIsExist == null)
                {
                    acc = _accountService.Insert(account);
                    
                    var _event = new Event()
                    {
                        AccountId = acc.Id,
                        EventTypeId = Constant.EventType.SigninSignoutOfWebPortal,
                        EventDetail = Constant.ResponseMessage.Signin,
                        Timestamp = DateTime.UtcNow,
                        Message = Constant.ResponseMessage.Signin
                    };

                    _eventService.Insert(_event);

                }
                else
                {
                    acc = CheckAccountIsExist;
                    
                }

                var accountAppliance = _accountApplianceceService.GetAccountApplianceByAccountId(acc.Id);
                if(accountAppliance != null && accountAppliance.Count() > 0)
                {
                    acc.AccountAppliances = accountAppliance;
                }

                response.Data = acc;
                response.Message = Constant.ResponseMessage.Success;
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
        public ResponseData<Account> UpdateInformation([FromBody] Account account)
        {
            try
            {
                var response = new ResponseData<Account>();
                var acc = _accountService.GetAccountById(account.Id);
                acc.FirstName = account.FirstName;
                acc.LastName = account.LastName;
                acc.Email = account.Email;
                _accountService.Update(acc);

                var accountAppliance = _accountApplianceceService.GetAccountApplianceByAccountId(acc.Id);
                if (accountAppliance != null && accountAppliance.Count() > 0)
                {
                    acc.AccountAppliances = accountAppliance;
                }

                response.Data = acc;
                response.Message = Constant.ResponseMessage.Success;
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
