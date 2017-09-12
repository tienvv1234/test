using System;
using System.Collections.Generic;
using System.Text;
using Airgap.Entity.Entities;
using Airgap.Entity;
using System.Linq;
using Airgap.Data.DTOEntities;

namespace Airgap.Service
{
    public class EventService : IEventService
    {
        private IRepository<Event> repoEvent;
        private IRepository<Account> repoAccount;
        private IRepository<EventType> repoEventType;
        public EventService(IRepository<EventType> repoEventType, IRepository<Event> repoEvent, IRepository<Account> repoAccount)
        {
            this.repoEvent = repoEvent;
            this.repoAccount = repoAccount;
            this.repoEventType = repoEventType;
        }

        public List<EventDTO> GetEventByApplianceId(long applianceId)
        {
            var result = (from e in repoEvent.Table
                          join a in repoAccount.Table on e.AccountId equals a.Id  into ps1
                          from a in ps1.DefaultIfEmpty()
                          join et in repoEventType.Table on e.EventTypeId equals et.Id into ps
                          from et in ps.DefaultIfEmpty()
                          where e.ApplianceId == applianceId
                          select new
                          {
                              eventLog = e,
                              account = a == null ? null : a,
                              eventType = et == null ? null : et
                          }).AsEnumerable().Select(a => new EventDTO
                          {
                              AccountId = a.eventLog.AccountId,
                              ApplianceId = a.eventLog.ApplianceId,
                              EventDetail = a.eventLog.EventDetail,
                              EventTypeId = a.eventLog.EventTypeId,
                              Message = a.eventLog.Message,
                              Timestamp = a.eventLog.Timestamp,
                              Account = a.account != null ? new AccountDTO(a.account) : null,
                              EventType = new EventType(a.eventType)
                              //AccountId = a != null && a.eventLog != null ? a.eventLog.AccountId : null,
                              //ApplianceId = a != null && a.eventLog != null ? a.eventLog.ApplianceId : null,
                              //EventDetail = a != null && a.eventLog != null ? a.eventLog.EventDetail : null,
                              //EventTypeId = a != null && a.eventLog != null ? a.eventLog.EventTypeId : null,
                              //Message = a != null && a.eventLog != null ? a.eventLog.Message : null,
                              //Timestamp = a != null && a.eventLog != null ? (DateTime?)a.eventLog.Timestamp : null,
                              //Account = a != null ? new AccountDTO(a.account) : null,
                              //EventType = a != null ? new EventType(a.eventType) : null
                          });

            return result.OrderByDescending(x => x.Timestamp).ToList();
        }

        public List<EventDTO> GetEventByDate(long applianceId, DateTime? dateFrom, DateTime? dateTo)
        {
            var result = (from e in repoEvent.Table
                          join a in repoAccount.Table on e.AccountId equals a.Id into ps1
                          from a in ps1.DefaultIfEmpty()
                          join et in repoEventType.Table on e.EventTypeId equals et.Id into ps
                          from et in ps.DefaultIfEmpty()
                          where e.ApplianceId == applianceId 
                          && (dateFrom == null || e.Timestamp >= dateFrom)
                          && (dateTo == null || e.Timestamp < dateTo)
                          select new
                          {
                              eventLog = e,
                              account = a == null ? null : a,
                              eventType = et == null ? null : et
                          }).AsEnumerable().Select(a => new EventDTO
                          {
                              AccountId = a.eventLog.AccountId,
                              ApplianceId = a.eventLog.ApplianceId,
                              EventDetail = a.eventLog.EventDetail,
                              EventTypeId = a.eventLog.EventTypeId,
                              Message = a.eventLog.Message,
                              Timestamp = a.eventLog.Timestamp,
                              Account = a.account != null ? new AccountDTO(a.account) : null,
                              EventType = a.eventType
                          });

            return result.OrderByDescending(x => x.Timestamp).ToList();
        }

        public Event Insert(Event _event)
        {
            return repoEvent.Insert(_event);
        }

        public void RemoveEventByApplianceId(long applianceId)
        {
            var events = repoEvent.FindAll(x => x.ApplianceId == applianceId);
            foreach (var item in events.ToList())
            {
                repoEvent.Delete(item);
            }
        }
    }
}
