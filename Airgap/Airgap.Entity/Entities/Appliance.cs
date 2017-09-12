using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("Appliance")]
    public partial class Appliance : BaseEntity
    {
        public Appliance()
        {
            AccountAppliances = new HashSet<AccountAppliance>();
            TimerSchedules = new HashSet<TimerSchedule>();
        }

        public long Id { get; set; }

        public long? AccountId { get; set; }

        [StringLength(50)]
        public string DeviceName { get; set; }

        [StringLength(16)]
        public string SerialNumber { get; set; }

        [StringLength(50)]
        public string OsVersion { get; set; }

        public bool? TimerEnabled { get; set; }

        public bool? GeoFenceEnabled { get; set; }

        public bool? IsConnected { get; set; }

        public int? TrustLevel { get; set; }

        public bool? Power { get; set; }

        public bool? Cellular { get; set; }

        public bool? WiFiInternet { get; set; }

        public bool? Wifi { get; set; }

        public bool? IsOn { get; set; }

        public bool? Status { get; set; }

        [StringLength(50)]
        public string Street1 { get; set; }

        [StringLength(50)]
        public string Street2 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public long? StateId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public decimal? TriggerMile { get; set; }

        public virtual ICollection<AccountAppliance> AccountAppliances { get; set; }

        public virtual State State { get; set; }

        public virtual ICollection<TimerSchedule> TimerSchedules { get; set; }
    }
}
