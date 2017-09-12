using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class AccountTokenMap
    {
        public AccountTokenMap(EntityTypeBuilder<AccountToken> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.Account).WithMany(e => e.AccountTokens).HasForeignKey(e => e.AccountId);            
        }
    }
}
