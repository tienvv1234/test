using Airgap.Entity;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Airgap.Service
{
    public interface IAccountService
    {
        Account Insert(Account Account);
        Account Update(Account Account);
        List<Account> GetAccounts();
        Account GetAccountByEmail(string email);
        Account GetAccountByFaceBookId(string appId);
        Account GetAccountById(long id);
        Account Signin(string email, string password);
        Account GetAccountByCustomerIdOfStripe(string id);
        Account GetAccountByPhoneNumber(string phoneNumber);
    }
}
