using Airgap.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airgap.Data.DTOEntities
{
    public class EventDTO
    {
        public EventDTO()
        {
        }

        public DateTime? Timestamp { get; set; }

        public long? ApplianceId { get; set; }

        public string Message { get; set; }

        public long? EventTypeId { get; set; }

        public string EventDetail { get; set; }

        public long? AccountId { get; set; }

        public AccountDTO Account { get; set; }

        public EventType EventType { get; set; }

        //public string PhoneNumber { get; set; }
    }
}
