using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public interface IToDoService
    {
        Task<List<ToDoItem>> GetItems(ApplicationUser user);

        Task<List<ToDoItem>> GetItemsByStatus(ApplicationUser user, ToDoStatuses itemStatus);

        Task<ToDoItem> GetItemByItemId(ApplicationUser user, int itemId);

        Task<ToDoItem> CreateItem(ApplicationUser user, ToDoItem item);

        Task<ToDoItem> UpdateItem(ApplicationUser user, ToDoItem item);

        Task DeleteItem(ApplicationUser user, int itemId);

        Task<ToDoItem> PatchItemStatus(ApplicationUser user, int itemId, ToDoStatuses itemStatus);
    }
}