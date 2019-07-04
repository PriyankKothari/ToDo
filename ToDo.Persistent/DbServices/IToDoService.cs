using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public interface IToDoService
    {
        Task<List<ToDoItem>> GetItems(int userId);

        Task<List<ToDoItem>> GetItemsByStatus(int userId, ToDoStatuses itemStatus);

        Task<ToDoItem> GetItemByItemId(int userId, int itemId);

        Task<ToDoItem> CreateItem(int userId, ToDoItem item);

        Task<ToDoItem> UpdateItem(int userId, ToDoItem item);

        Task DeleteItem(int userId, int itemId);

        Task<ToDoItem> PatchItemStatus(int userId, int itemId, ToDoStatuses itemStatus);
    }
}