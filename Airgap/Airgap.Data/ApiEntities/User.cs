using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.ApiEntities
{
    public class User
    {
        public List<AccountDTO> lAccountDTO { get; set; }
        public List<ApplianceDTO> lAppliance { get; set; }
    }
}
