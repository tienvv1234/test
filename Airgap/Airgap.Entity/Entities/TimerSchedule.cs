using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("TimerSchedule")]
    public partial class TimerSchedule : BaseEntity
    {
        public long? ApplianceId { get; set; }

        public long? TimerTypeId { get; set; }

        [Required]
        [StringLength(60)]
        public string ActiveValues { get; set; }

        public virtual Appliance Appliance { get; set; }

        public virtual TimerType TimerType { get; set; }
    }
}
