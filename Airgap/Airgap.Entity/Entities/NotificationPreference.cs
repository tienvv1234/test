using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("NotificationPreference")]
    public partial class NotificationPreference : BaseEntity
    {
        public long EventTypeId { get; set; }

        public long AccountId { get; set; }

        public long ApplianceId { get; set; }

        public virtual EventType EventType { get; set; }
    }
}
