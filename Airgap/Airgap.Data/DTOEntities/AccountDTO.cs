using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class AccountDTO
    {
        public AccountDTO() { }

        public AccountDTO(Account account) {
            this.Id = account.Id;
            this.Email = account.Email;
            this.FirstName = account.FirstName;
            this.IsAdmin = account.IsAdmin;
            this.IsVerified = account.IsVerified;
            this.LastName = account.LastName;
            this.PhoneNumber = account.PhoneNumber;
        }

        public string DeviceName { get; set; }

        public bool? IsVerifiedMobile { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public long? Id { get; set; }

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

        public bool? NetWorkStatusChange { get; set; }

        public bool? HomePowerLoss { get; set; }

        public bool? ISPOutage { get; set; }

    }
}
