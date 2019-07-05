using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Extensions;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;
using Microsoft.ServiceBus.Messaging;

namespace ToDo.Persistent.DbServices
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoDbContext _toDoDbContext;
        private readonly QueueClient _queueClient;

        private ToDoService(IToDoDbContext toDoDbContext, string serviceBusConnectionString, string queueName)
        {
            this._toDoDbContext = toDoDbContext;
            this._queueClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, queueName);
        }

        public async Task<List<ToDoItem>> GetItems(int userId)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.Where(td => td.UserId.Equals(userId)).AsNoTracking()
                    .ToListAsync(CancellationToken.None);
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while getting your ToDo Items: {exception}");
            }
        }

        public async Task<List<ToDoItem>> GetItemsByStatus(int userId, ToDoStatuses itemStatus)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.Where(td => td.UserId.Equals(userId))
                    .Where(td => td.ItemStatus.Equals(itemStatus)).AsNoTracking().ToListAsync(CancellationToken.None);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    $"Something went wrong while getting your ToDo Items by status '{itemStatus.ToStringValue()}': {exception}");
            }
        }

        public async Task<ToDoItem> GetItemByItemId(int userId, int itemId)
        {
            try
            {
                return await this._toDoDbContext.ToDoItems.SingleOrDefaultAsync(
                    td => td.UserId.Equals(userId) && td.ItemId.Equals(itemId), CancellationToken.None);
            }
            catch (Exception exception)
            {
                throw new Exception($"Something went wrong while getting your ToDo Item (Id - {itemId}): {exception}");
            }
        }

        public async Task<ToDoItem> CreateItem(int userId, ToDoItem item)
        {
            try
            {
                #region Create ToDo Item

                var itemToCreate = new ToDoItem
                {
                    ItemId = item.ItemId,
                    ItemTitle = item.ItemTitle,
                    ItemStatus = item.ItemStatus,
                    UserId = userId,
                    ItemDueDateTime = item.ItemDueDateTime
                };

                this._toDoDbContext.ToDoItems.Add(itemToCreate);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                try
                {
                    await this._queueClient.SendAsync(new BrokeredMessage(string.Empty));
                }
                catch (MessagingException exception)
                {
                    throw exception.IsTransient
                        ? new Exception(exception.Message)
                        : new MessagingException(exception.Message);
                }

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

        public async Task<ToDoItem> UpdateItem(int userId, ToDoItem item)
        {
            try
            {
                #region Update ToDo Item

                var itemToUpdate =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(td =>
                        td.UserId.Equals(userId) && td.ItemId.Equals(item.ItemId));

                if (itemToUpdate == null)
                    return null;

                itemToUpdate.ItemTitle = item.ItemTitle;
                itemToUpdate.ItemStatus = item.ItemStatus;
                itemToUpdate.ItemDueDateTime = item.ItemDueDateTime;

                this._toDoDbContext.ToDoItems.Update(itemToUpdate);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                try
                {
                    await this._queueClient.SendAsync(new BrokeredMessage(string.Empty));
                }
                catch (MessagingException exception)
                {
                    throw exception.IsTransient
                        ? new Exception(exception.Message)
                        : new MessagingException(exception.Message);
                }

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

        public async Task<ToDoItem> PatchItemStatus(int userId, int itemId, ToDoStatuses itemStatus)
        {
            try
            {
                #region Patch ToDo Item Status

                var itemToPatch =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(
                        td => td.UserId.Equals(userId) && td.ItemId.Equals(itemId));

                if (itemToPatch == null)
                    return null;

                itemToPatch.ItemStatus = itemStatus;

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);

                #endregion

                #region Push Message to Azure Service Bus

                try
                {
                    await this._queueClient.SendAsync(new BrokeredMessage(string.Empty));
                }
                catch (MessagingException exception)
                {
                    throw exception.IsTransient
                        ? new Exception(exception.Message)
                        : new MessagingException(exception.Message);
                }

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

        public async Task DeleteItem(int userId, int itemId)
        {
            try
            {
                #region Delete ToDo Item
                var itemToDelete =
                    this._toDoDbContext.ToDoItems.SingleOrDefault(
                        td => td.UserId.Equals(userId) && td.ItemId.Equals(itemId));

                if (itemToDelete == null)
                    return;

                this._toDoDbContext.ToDoItems.Remove(itemToDelete);

                await this._toDoDbContext.SaveChangesAsync(true, CancellationToken.None);
                #endregion

                #region Push Message to Azure Service Bus

                try
                {
                    await this._queueClient.SendAsync(new BrokeredMessage(string.Empty));
                }
                catch (MessagingException exception)
                {
                    throw exception.IsTransient
                        ? new Exception(exception.Message)
                        : new MessagingException(exception.Message);
                }

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