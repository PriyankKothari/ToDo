using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public interface IEventStoreService
    {
        Task<List<Event>> GetEvents();

        Task<Event> CreateEvent(Event newEvent);
    }
}