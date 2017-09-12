using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class TimerScheduleMap
    {
        public TimerScheduleMap(EntityTypeBuilder<TimerSchedule> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasOne(e => e.Appliance).WithMany(e => e.TimerSchedules).HasForeignKey(e => e.ApplianceId);            
            entityBuilder.HasOne(e => e.TimerType).WithMany(e => e.TimerSchedules).HasForeignKey(e => e.TimerTypeId);            
        }
    }
}
