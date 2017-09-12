using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class ProfileViewModels
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ExistingPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReTypePassword { get; set; }
        public string PhoneNumber { get; set; }
    }
}
