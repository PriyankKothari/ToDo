using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.Persistent.DbServices
{
    public interface IToDoService
    {
        Task<List<ToDoItem>> GetItems(IdentityUser user);

        Task<List<ToDoItem>> GetItemsByStatus(IdentityUser user, ToDoStatuses itemStatus);

        Task<ToDoItem> GetItemByItemId(IdentityUser user, int itemId);

        Task<ToDoItem> CreateItem(IdentityUser user, ToDoItem item);

        Task<ToDoItem> UpdateItem(IdentityUser user, ToDoItem item);

        Task DeleteItem(IdentityUser user, int itemId);

        Task<ToDoItem> PatchItemStatus(IdentityUser user, int itemId, ToDoStatuses itemStatus);
    }
}