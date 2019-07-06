using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDo.Persistent.DbContexts;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.Tests.TestHelpers
{
    public class ToDoDbContextBuilder
    {
        private DbConnection _sqlConnection;
        private readonly List<ToDoItem> _toDoItems;
        private readonly List<Event> _events;

        public ToDoDbContextBuilder()
        {
            this._toDoItems = new List<ToDoItem>();
            this._events = new List<Event>();
        }

        public ToDoDbContextBuilder UseInMemorySqlite()
        {
            this._sqlConnection = new SqliteConnection("DataSource=:memory:");
            return this;
        }

        public ToDoDbContext Build()
        {
            if (this._sqlConnection == null)
                throw new Exception("SqlConnection has not been initialized");

            if (this._sqlConnection.State == ConnectionState.Closed)
                this._sqlConnection.Open();

            var dbContext =
                new ToDoDbContext(new DbContextOptionsBuilder<ToDoDbContext>().UseSqlite(_sqlConnection).Options);
            dbContext.Database.EnsureCreated();

            dbContext.ToDoItems.AddRange(this._toDoItems);
            dbContext.Events.AddRange(this._events);

            dbContext.SaveChanges();
            return dbContext;
        }

        public ToDoDbContextBuilder WithToDoItem(int itemId, string itemTitle, ToDoStatuses itemStatus,
            DateTimeOffset? itemDueOn, Guid userId)
        {
            var toDoItem = new ToDoItem
            {
                ItemId = itemId,
                ItemTitle = itemTitle,
                ItemStatus = itemStatus,
                ItemDueOn = itemDueOn,
                UserId = userId
            };

            this._toDoItems.Add(toDoItem);
            return this;
        }

        public ToDoDbContextBuilder WithEvent(int eventId, string eventType, int aggregateId, string aggregateName,
            string eventPayLoad, DateTime eventCreateDateTime)
        {
            var @event = new Event
            {
                EventType = eventType,
                AggregateId = aggregateId,
                AggregateName = aggregateName,
                EventPayLoad = eventPayLoad,
                EventCreateDateTime = eventCreateDateTime
            };

            this._events.Add(@event);
            return this;
        }
    }
}