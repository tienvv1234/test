using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class NotificationPreferenceMap
    {
        public NotificationPreferenceMap(EntityTypeBuilder<NotificationPreference> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.EventType).WithMany(e => e.NotificationPreferences).HasForeignKey(e => e.EventTypeId);            
        }
    }
}
