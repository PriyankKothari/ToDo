using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.Persistent.Entities;

namespace ToDo.Persistent.EntityFramework
{
    public interface IToDoDbContext
    {
        DbSet<ToDoItem> ToDoItems { get; set; }

        DbSet<Event> Events { get; set; }

        int SaveChanges(bool acceptAllChangesOnSuccess = true);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}