using Airgap.Data.DTOEntities;
using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Service
{
    public interface IEventService
    {
        Event Insert(Event _event);
        List<EventDTO> GetEventByApplianceId(long applianceId);
        List<EventDTO> GetEventByDate(long applianceId, DateTime? dateFrom, DateTime? dateTo);
        void RemoveEventByApplianceId(long applianceId);
    }
}
