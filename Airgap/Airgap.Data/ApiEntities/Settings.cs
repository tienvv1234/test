using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.ApiEntities
{
    public class Settings
    {
        public List<State> States { get; set; }

        public List<Appliance> Appliances { get; set; }

    }
}
