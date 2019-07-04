using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ToDo.Domain.Extensions;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbContexts
{
    public class ToDoDbContext : DbContext, IToDoDbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {

        }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ToDoItem
            modelBuilder.Entity<ToDoItem>().ToTable("Items", schema: "ToDo").HasKey(td => td.ItemId);
            modelBuilder.Entity<ToDoItem>().Property(td => td.ItemId).ValueGeneratedOnAdd();
            modelBuilder.Entity<ToDoItem>().Property(td => td.ItemTitle).HasMaxLength(100);
            modelBuilder.Entity<ToDoItem>().Property(td => td.ItemStatus).HasConversion(
                status => status.ToStringValue(), status => status.ToEnumeration<ToDoStatuses>(true, true));

            // Event
            modelBuilder.Entity<Event>().ToTable("Events", schema: "EventStore").HasKey(ev => ev.EventId);
            modelBuilder.Entity<Event>().Property(ev => ev.EventId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Event>().Property(ev => ev.EventType).HasMaxLength(100);
            modelBuilder.Entity<Event>().Property(ev => ev.AggregateName).HasMaxLength(100);
            modelBuilder.Entity<Event>().Property(ev => ev.EventPayLoad).HasConversion(
                ev => JsonConvert.SerializeObject(ev,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                ev => JsonConvert.DeserializeObject<string>(ev,
                    new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));

            base.OnModelCreating(modelBuilder);
        }
    }
}