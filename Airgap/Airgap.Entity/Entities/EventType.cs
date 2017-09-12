using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("EventType")]
    public partial class EventType : BaseEntity
    {
        public EventType()
        {
            Events = new HashSet<Event>();
            NotificationPreferences = new HashSet<NotificationPreference>();
        }

        public EventType(EventType eventType)
        {
            this.Id = eventType.Id;
            this.EventTypeName = eventType.EventTypeName;
        }


        [Required]
        [StringLength(60)]
        public string EventTypeName { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<NotificationPreference> NotificationPreferences { get; set; }
    }
}
