using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("Notification")]
    public partial class Notification : BaseEntity
    {
        public long? AccountId { get; set; }

        public long? EventId { get; set; }

        public long? ApplianceId { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual Event Event { get; set; }
    }
}
