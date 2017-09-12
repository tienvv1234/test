using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.WebUi.Models
{
    public class AirViewModels
    {
        public List<ApplianceDTO> lAppliance { get; set; }
        public ApplianceDTO SelectedAppliance { get; set; }
        public List<AccountDTO> lAccountDTO { get; set; }
        public Dictionary<long,string[]> lTimerSchedule { get; set; }
        //public string State { get; set; }
        public List<SelectListItem> States { get; set; }


        public string StreetName { get; set; }

        public string StreetName2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string UseAddressFromTelit { get; set; }
    }
}
