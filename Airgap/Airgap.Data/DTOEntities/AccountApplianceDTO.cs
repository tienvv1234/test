using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class AccountApplianceDTO
    {
        public AccountApplianceDTO() { }

        public AccountApplianceDTO (AccountAppliance accountAppliance)
        {
            this.ApplianceId = accountAppliance.ApplianceId;
            this.AccountId = accountAppliance.AccountId;
            this.Lat = accountAppliance.Lat;
            this.Lon = accountAppliance.Lon;
            this.IsQRCodeScaned = accountAppliance.IsQRCodeScaned;
            this.IsVerified = accountAppliance.IsVerified;
            this.AirGapVersion = accountAppliance.AirGapVersion;
            this.ApplianceDTO = new ApplianceDTO(accountAppliance.Appliance);
        }

        public long? ApplianceId { get; set; }

        public long? AccountId { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public bool? IsQRCodeScaned { get; set; }

        public bool? IsVerified { get; set; }

        public string AirGapVersion { get; set; }

        public virtual AccountDTO AccountDTO { get; set; }

        public virtual ApplianceDTO ApplianceDTO { get; set; }
    }
}
