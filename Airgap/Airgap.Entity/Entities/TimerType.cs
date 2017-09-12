using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("TimerType")]
    public partial class TimerType : BaseEntity
    {
        public TimerType()
        {
            TimerSchedules = new HashSet<TimerSchedule>();
        }

        [Required]
        [StringLength(50)]
        public string TimerTypeName { get; set; }

        public virtual ICollection<TimerSchedule> TimerSchedules { get; set; }
    }
}
