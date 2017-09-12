using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("AccountAppliance")]
    public partial class AccountAppliance : BaseEntity
    {
        public long? ApplianceId { get; set; }

        public long? AccountId { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public bool? IsQRCodeScaned { get; set; }

        public bool? IsVerified { get; set; }

        public string IdentifierForVendor { get; set; }

        public string DeviceName { get; set; }

        public string PhoneType { get; set; }

        public string DeviceToken { get; set; }

        public string SubscriptionId { get; set; }

        [StringLength(50)]
        public string AirGapVersion { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }

        [JsonIgnore]
        public virtual Appliance Appliance { get; set; }
    }
}
