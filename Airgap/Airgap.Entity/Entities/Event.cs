using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("Event")]
    public partial class Event : BaseEntity
    {
        public Event()
        {
            Notifications = new HashSet<Notification>();
        }


        public DateTime Timestamp { get; set; }

        public long? ApplianceId { get; set; }

        [StringLength(50)]
        public string Message { get; set; }

        public long? EventTypeId { get; set; }

        [StringLength(50)]
        public string EventDetail { get; set; }

        public long? AccountId { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
