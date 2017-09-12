using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class UserViewModels
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string hdAccountId { get; set; }
        public bool? Verify { get; set; }
        public List<AccountDTO> lAccount { get; set; }
        public List<ApplianceDTO> lAppliance { get; set; }
        public ApplianceDTO SelectedAppliance { get; set; }
    }
}
