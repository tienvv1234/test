using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class TimerTypeMap
    {
        public TimerTypeMap(EntityTypeBuilder<TimerType> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
        }
    }
}
