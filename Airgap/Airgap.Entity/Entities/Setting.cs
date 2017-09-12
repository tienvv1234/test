using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("Setting")]
    public partial class Setting : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string SettingName { get; set; }

        [Required]
        [StringLength(200)]
        public string SettingValue { get; set; }
    }
}
