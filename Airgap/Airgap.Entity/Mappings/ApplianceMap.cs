using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class ApplianceMap
    {
        public ApplianceMap(EntityTypeBuilder<Appliance> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.State).WithMany(e => e.Appliances).HasForeignKey(e => e.StateId);

        }
    }
}
