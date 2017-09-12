using Airgap.Entity;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Airgap.Service
{
    public class AccountService : IAccountService
    {
        private IRepository<Account> repoAccount;

        public AccountService(IRepository<Account> repoAccount)
        {
            this.repoAccount = repoAccount;
        }
        public Account Insert(Account Account)
        {
            return repoAccount.Insert(Account);
        }

        public Account Update(Account Account)
        {
            return repoAccount.Update(Account);
        }
        public List<Account> GetAccounts()
        {
            return repoAccount.GetAll().ToList();
        }

        public Account GetAccountByEmail(string email)
        {
            return repoAccount.Find(x => x.Email == email);
        }

        public Account GetAccountByFaceBookId(string appId)
        {
            return repoAccount.Find(x => x.FaceBookId == appId);
        }

        public Account Signin(string email, string password)
        {

            return repoAccount.Find(x => x.Email == email && x.Password == password);
        }

        public Account GetAccountById(long id)
        {
            return repoAccount.Find(x => x.Id == id);
        }

        public Account GetAccountByCustomerIdOfStripe(string id)
        {
            return repoAccount.Find(x => x.CustomerIdStripe == id);
        }

        public Account GetAccountByPhoneNumber(string phoneNumber)
        {
            return repoAccount.Find(x => x.PhoneNumber == phoneNumber);
        }
    }
}
