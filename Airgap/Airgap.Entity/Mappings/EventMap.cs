using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class EventMap
    {
        public EventMap(EntityTypeBuilder<Event> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.EventType).WithMany(e => e.Events).HasForeignKey(e => e.EventTypeId);            
        }
    }
}
