using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;
using ToDo.Tests.TestHelpers;

namespace ToDo.Tests.Services
{
    [TestClass]
    public class EventStoreServiceTests
    {
        private IEventStoreService _eventStoreService;
        private readonly IdentityUser _testUser = new IdentityUser("TestUser");

        [TestMethod]
        public void ShouldReturn_NoEvents_When_NoEventsExists()
        {
            // set up
            this._eventStoreService = new EventStoreService(new ToDoDbContextBuilder().UseInMemorySqlite().Build());

            // test
            var result = this._eventStoreService.GetEvents().Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ShouldReturn_AllEvents_When_EventsExist()
        {
            //set up
            Guid eventId = Guid.NewGuid();

            var @event = new Event
            {
                EventId = eventId,
                EventType = "ToDoItemAdded",
                AggregateId = Guid.Parse(_testUser.Id),
                AggregateName = "ToDoItem",
                EventPayLoad = "Json String as Event Payload",
                EventCreateDateTime = DateTime.Now
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite().WithEvent(@event.EventId,
                @event.EventType, @event.AggregateId, @event.AggregateName, @event.EventPayLoad,
                @event.EventCreateDateTime).Build();

            this._eventStoreService = new EventStoreService(mockDatabaseContext);

            // test
            var result = this._eventStoreService.GetEvents().Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(@event.EventType, result[0].EventType);
            Assert.AreEqual(@event.AggregateId, result[0].AggregateId);
            Assert.AreEqual(@event.AggregateName, result[0].AggregateName);
            Assert.AreEqual(@event.EventPayLoad, result[0].EventPayLoad);
            Assert.AreEqual(@event.EventCreateDateTime, result[0].EventCreateDateTime);
        }

        [TestMethod]
        public void ShouldCreate_Event_When_EventIsValid()
        {
            //set up
            var eventToCreate = new Event
            {
                EventId = Guid.NewGuid(),
                EventType = "ToDoItemAdded",
                AggregateId = Guid.Parse(_testUser.Id),
                AggregateName = "ToDoItem",
                EventPayLoad = "Json String as Event Payload",
                EventCreateDateTime = DateTime.Now
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite().Build();

            this._eventStoreService = new EventStoreService(mockDatabaseContext);

            // test
            var @event = this._eventStoreService.CreateEvent(eventToCreate).Result;

            // assert
            Assert.IsNotNull(@event);
            Assert.AreEqual(eventToCreate.EventId, @event.EventId);
            Assert.AreEqual(eventToCreate.EventType, @event.EventType);
            Assert.AreEqual(eventToCreate.AggregateId, @event.AggregateId);
            Assert.AreEqual(eventToCreate.AggregateName, @event.AggregateName);
            Assert.AreEqual(eventToCreate.EventPayLoad, @event.EventPayLoad);
            Assert.AreEqual(eventToCreate.EventCreateDateTime, @event.EventCreateDateTime);
        }
    }
}