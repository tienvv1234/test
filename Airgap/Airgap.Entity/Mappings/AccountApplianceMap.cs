using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class AccountApplianceMap
    {
        public AccountApplianceMap(EntityTypeBuilder<AccountAppliance> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.Account).WithMany(e => e.AccountAppliances).HasForeignKey(e => e.AccountId);            
            entityBuilder.HasOne(e => e.Appliance).WithMany(e => e.AccountAppliances).HasForeignKey(e => e.ApplianceId);            
        }
    }
}
