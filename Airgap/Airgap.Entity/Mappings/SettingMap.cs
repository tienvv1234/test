using Airgap.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airgap.Entity.Mappings
{
    public class SettingMap
    {
        public SettingMap(EntityTypeBuilder<Setting> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
        }
    }
}
