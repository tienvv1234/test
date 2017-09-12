using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Airgap.Entity.Entities
{
    [Table("Account")]
    public partial class Account : BaseEntity
    {
        public Account()
        {
            AccountAppliances = new HashSet<AccountAppliance>();
            AccountTokens = new HashSet<AccountToken>();
            PasswordHistories = new HashSet<PasswordHistory>();
        }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public bool? IsVerified { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public bool? IsAdmin { get; set; }

        [StringLength(500)]
        public string LastPasswords { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(20)]
        public string FaceBookId { get; set; }

        public string CustomerIdStripe { get; set; }
        

        public virtual ICollection<AccountAppliance> AccountAppliances { get; set; }

        public virtual ICollection<AccountToken> AccountTokens { get; set; }

        public virtual ICollection<PasswordHistory> PasswordHistories { get; set; }
    }
}
