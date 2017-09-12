using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class DashBoardViewModels
    {
        public List<ApplianceDTO> lAppliance { get; set; }
        public List<EventDTO> lEvent { get; set; }
        public ApplianceDTO SelectedAppliance { get; set; }
        public List<AccountApplianceDTO> lAcccountAppliance { get; set; }
        public List<AccountDTO> lAccountsDTO { get; set; }
        public DateTime? Timer { get; set; }
        public bool? IOTIsConnected { get; set; }
        public List<StripePlan> lPlan { get; set; }
        public bool IsApplianceConnected { get; set; }
        public bool CancelAtEnd { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
