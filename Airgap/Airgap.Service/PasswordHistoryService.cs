using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;

namespace Airgap.Service
{
    public class PasswordHistoryService : IPasswordHistoryService
    {
        private IRepository<PasswordHistory> repoPasswordHistory;
        public PasswordHistoryService(IRepository<PasswordHistory> repoPasswordHistory)
        {
            this.repoPasswordHistory = repoPasswordHistory;
        }

        public PasswordHistory Insert(PasswordHistory passwordHistory)
        {
            return repoPasswordHistory.Insert(passwordHistory);
        }
    }
}
