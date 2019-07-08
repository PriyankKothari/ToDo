using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Extensions;
using ToDo.Domain.Helpers;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;
using ToDo.ServiceBus.MessageSenders;

namespace ToDo.Persistent.DbServices
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoDbContext _toDoDbContext;
        private readonly IMessageSender _messageSender;

        public ToDoService(IToDoDbContext toDoDbContext, IMessageSender messageSender)
        {
            this._toDoDbContext = toDoDbContext;
            this._messageSender = messageSender;
        }

        public async Task<List<ToDoItem>> GetItems(IdentityUser user)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.Where(td => td.UserId.Equals(Guid.Parse(user.Id)))
                    .AsNoTracking().ToListAsync(CancellationToken.None);
            }

            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while getting your ToDo Items: {exception}");
            }
        }

        public async Task<List<ToDoItem>> GetItemsByStatus(IdentityUser user, ToDoStatuses itemStatus)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.Where(td => td.UserId.Equals(Guid.Parse(user.Id)))
                    .Where(td => td.ItemStatus.Equals(itemStatus)).AsNoTracking().ToListAsync(CancellationToken.None);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    $"Something went wrong while getting your ToDo Items by status '{itemStatus.ToStringValue()}': {exception}");
            }
        }

        public async Task<ToDoItem> GetItemByItemId(IdentityUser user, int itemId)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.SingleOrDefaultAsync(
                    td => td.UserId.Equals(Guid.Parse(user.Id)) && td.ItemId.Equals(itemId), CancellationToken.None);
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while getting your ToDo Item (Id - {itemId}): {exception}");
            }
        }

        public async Task<ToDoItem> CreateItem(IdentityUser user, ToDoItem item)
        {
            try
            {
                #region Create ToDo Item

                var itemToCreate = new ToDoItem
                {
                    ItemId = item.ItemId,
                    ItemTitle = item.ItemTitle,
                    ItemStatus = item.ItemStatus,
                    UserId = Guid.Parse(user.Id),
                    ItemDueOn = item.ItemDueOn
                };

                this._toDoDbContext.ToDoItems.Add(itemToCreate);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                var createdEvent = new Event
                {
                    EventId = Guid.NewGuid(),
                    EventType = "ItemAdded",
                    AggregateId = Guid.Parse(user.Id),
                    AggregateName = "ToDoItem",
                    EventCreateDateTime = DateTime.Now,
                    EventPayLoad = item.ObjectToJson()
                };
                //await this._messageSender.SendMessage(createdEvent.ObjectToJson());

                #endregion

                return itemToCreate;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception($"Something went wrong on the Database while creating a ToDo Item: {exception.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while creating a ToDo Item: {exception.Message}");
            }
        }

        public async Task<ToDoItem> UpdateItem(IdentityUser user, ToDoItem item)
        {
            try
            {
                #region Update ToDo Item

                var itemToUpdate =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(td =>
                        td.UserId.Equals(Guid.Parse(user.Id)) && td.ItemId.Equals(item.ItemId));

                if (itemToUpdate == null)
                    return null;

                itemToUpdate.ItemTitle = item.ItemTitle;
                itemToUpdate.ItemStatus = item.ItemStatus;
                itemToUpdate.ItemDueOn = item.ItemDueOn;

                this._toDoDbContext.ToDoItems.Update(itemToUpdate);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                var updatedEvent = new Event
                {
                    EventId = Guid.NewGuid(),
                    EventType = "ItemUpdated",
                    AggregateId = Guid.Parse(user.Id),
                    AggregateName = "ToDoItem",
                    EventCreateDateTime = DateTime.Now,
                    EventPayLoad = item.ObjectToJson()
                };
                //await this._messageSender.SendMessage(updatedEvent.ObjectToJson());

                #endregion

                return itemToUpdate;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception($"Something went wrong on the Database while updating a ToDo Item: {exception.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while updating a ToDo Item: {exception.Message}");
            }
        }

        public async Task<ToDoItem> PatchItemStatus(IdentityUser user, int itemId, ToDoStatuses itemStatus)
        {
            try
            {
                #region Patch ToDo Item Status

                var itemToPatch =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(
                        td => td.UserId.Equals(Guid.Parse(user.Id)) && td.ItemId.Equals(itemId));

                if (itemToPatch == null)
                    return null;

                itemToPatch.ItemStatus = itemStatus;

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                var patchedEvent = new Event
                {
                    EventId = Guid.NewGuid(),
                    EventType = "ItemPatched",
                    AggregateId = Guid.Parse(user.Id),
                    AggregateName = "ToDoItem",
                    EventCreateDateTime = DateTime.Now,
                    EventPayLoad = $"{{ItemStatus: {itemStatus}}}"
                };
                //await this._messageSender.SendMessage(patchedEvent.ObjectToJson());

                #endregion

                return itemToPatch;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception($"Something went wrong on the Database while updating a ToDo Item Status: {exception.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while updating a ToDo Item Status: {exception.Message}");
            }
        }

        public async Task DeleteItem(IdentityUser user, int itemId)
        {
            try
            {
                #region Delete ToDo Item
                var itemToDelete =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(
                        td => td.UserId.Equals(Guid.Parse(user.Id)) && td.ItemId.Equals(itemId));

                if (itemToDelete == null)
                    return;

                this._toDoDbContext.ToDoItems.Remove(itemToDelete);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);
                #endregion

                #region Push Message to Azure Service Bus

                var deletedEvent = new Event
                {
                    EventId = Guid.NewGuid(),
                    EventType = "ItemDeleted",
                    AggregateId = Guid.Parse(user.Id),
                    AggregateName = "ToDoItem",
                    EventCreateDateTime = DateTime.Now,
                    EventPayLoad = itemToDelete.ObjectToJson()
                };
                //await this._messageSender.SendMessage(deletedEvent.ObjectToJson());

                #endregion
            }
            catch (DbException exception)
            {
                throw new Exception($"Something went wrong on the Database while deleting a ToDo Item: {exception.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while deleting a ToDo Item: {exception.Message}");
            }
        }
    }
}