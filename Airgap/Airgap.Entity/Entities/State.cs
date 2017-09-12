using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("State")]
    public partial class State : BaseEntity
    {
        public State()
        {
            Appliances = new HashSet<Appliance>();
        }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Appliance> Appliances { get; set; }
    }
}
