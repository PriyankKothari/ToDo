using Microsoft.EntityFrameworkCore;
using ToDo.Persistent.Entities;

namespace ToDo.Persistent.EntityFramework
{
    public class ToDoDbContext : DbContext, IToDoDbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {

        }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}