using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.ApiEntities
{
    public class Dashboard
    {
        public List<AccountApplianceDTO> AccountAppliance { get; set; }

        public List<EventDTO> Events { get; set; }

        public TimerScheduleDTO TimerScheduleDTO { get; set; }

        public List<AccountDTO> ListAccountsDTO { get; set; }

        public List<ApplianceDTO> ApplianceDTO { get; set; }

        public List<StripePlan> Plans { get; set; }

        public bool IsIOTConnected { get; set; }

        public bool IsApplianceConnected { get; set; }
        public bool CancelAtEnd { get; set; }

        public int? PercentOff { get; set; }
        public int? AmountOff { get; set; }

        public ApplianceDTO AppDTO { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}
