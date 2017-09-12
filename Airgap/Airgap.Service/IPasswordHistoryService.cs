using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IPasswordHistoryService 
    {
        PasswordHistory Insert(PasswordHistory passwordHistory);
    }
}
