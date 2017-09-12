using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class StateMap
    {
        public StateMap(EntityTypeBuilder<State> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
        }
    }
}
