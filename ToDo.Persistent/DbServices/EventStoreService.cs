using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public class EventStoreService : IEventStoreService
    {
        private readonly IToDoDbContext _toDoDbContext;

        private EventStoreService(IToDoDbContext toDoDbContext)
        {
            this._toDoDbContext = toDoDbContext;
        }

        public async Task<List<Event>> GetEvents()
        {
            return await this._toDoDbContext.Events.AsNoTracking().ToListAsync(CancellationToken.None);
        }

        public async Task<Event> CreateEvent(Event createEvent)
        {
            try
            {
                var eventToCreate = new Event
                {
                    EventId = createEvent.EventId,
                    EventType = createEvent.EventType,
                    AggregateId = createEvent.AggregateId,
                    AggregateName = createEvent.AggregateName,
                    EventCreateDateTime = createEvent.EventCreateDateTime,
                    EventPayLoad = createEvent.EventPayLoad
                };

                this._toDoDbContext.Events.Add(eventToCreate);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                return eventToCreate;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception("Something went wrong while creating an Event: ", exception);
            }
        }
    }
}