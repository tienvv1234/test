using Airgap.Entity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class SetUpViewModels
    {
        [Required]
        public string SerialNumber { get; set; }
        [Required]
        public string StreetName { get; set; }

        public string StreetName2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Zipcode { get; set; }

        public string UseAddressFromTelit{ get; set; }

        public List<SelectListItem> States {get; set;}

    }
}
