using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("PasswordHistory")]
    public partial class PasswordHistory : BaseEntity
    {
        public long? AccountId { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public DateTime? CreatedOn { get; set; }

        public virtual Account Account { get; set; }
    }
}
