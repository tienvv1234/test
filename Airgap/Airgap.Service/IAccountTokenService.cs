using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IAccountTokenService
    {
        AccountToken Insert(AccountToken accountToken);
        AccountToken Update(AccountToken accountToken);
        AccountToken GetAccountTokenByAccountId(long id);
        AccountToken GetAccountTokenByToken(string token);
        void Remove(AccountToken accountToken);
    }
}
