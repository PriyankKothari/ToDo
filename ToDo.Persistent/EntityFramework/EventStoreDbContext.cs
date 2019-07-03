using Microsoft.EntityFrameworkCore;
using ToDo.Persistent.Entities;

namespace ToDo.Persistent.EntityFramework
{
    public class EventStoreDbContext : DbContext, IEventStoreDbContext
    {
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}