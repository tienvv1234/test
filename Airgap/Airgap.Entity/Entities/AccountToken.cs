using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("AccountToken")]
    public partial class AccountToken : BaseEntity
    {
        public long? AccountId { get; set; }

        [StringLength(50)]
        public string Token { get; set; }

        public DateTime? Timestamp { get; set; }

        public bool? IsSignupToken { get; set; }
        [JsonIgnore]
        public virtual Account Account { get; set; }
    }
}
