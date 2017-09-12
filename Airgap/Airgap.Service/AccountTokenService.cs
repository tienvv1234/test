using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;

namespace Airgap.Service
{
    public class AccountTokenService : IAccountTokenService
    {
        private IRepository<AccountToken> repoAccountToken;
        public AccountTokenService(IRepository<AccountToken> repoAccountToken)
        {
            this.repoAccountToken = repoAccountToken;
        }

        public AccountToken GetAccountTokenByAccountId(long id)
        {
            return repoAccountToken.Find(x => x.AccountId == id);
        }

        public AccountToken GetAccountTokenByToken(string token)
        {
            return repoAccountToken.Find(x => x.Token == token);
        }

        public AccountToken Insert(AccountToken accountToken)
        {
            return repoAccountToken.Insert(accountToken);
        }

        public void Remove(AccountToken accountToken)
        {
            repoAccountToken.Delete(accountToken);
        }

        public AccountToken Update(AccountToken accountToken)
        {
            return repoAccountToken.Update(accountToken);
        }
    }
}
