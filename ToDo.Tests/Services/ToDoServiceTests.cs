using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;
using ToDo.Persistent.DbServices;
using ToDo.ServiceBus.MessageSenders;
using ToDo.Tests.TestHelpers;

namespace ToDo.Tests.Services
{
    [TestClass]
    public class ToDoServiceTests
    {
        private IToDoService _toDoService;
        private readonly IEventStoreService _eventStoreService;
        private readonly IdentityUser _testUser = new IdentityUser("TestUser");

        public ToDoServiceTests()
        {
            this._eventStoreService = new EventStoreService(new ToDoDbContextBuilder().UseInMemorySqlite().Build());
        }


        [TestMethod]
        public void ShouldReturn_NoToDoItems_When_NoToDoItemExists()
        {
            // set up
            var messageSender = new Mock<IMessageSender>();
            messageSender.Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService =
                new ToDoService(new ToDoDbContextBuilder().UseInMemorySqlite().Build(), this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItems(_testUser).Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldReturn_AllToDoItems_When_ToDoItemsExist()
        {
            //set up
            var toDoItem = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite().WithToDoItem(toDoItem.ItemId,
                toDoItem.ItemTitle, toDoItem.ItemStatus, toDoItem.ItemDueOn, toDoItem.UserId).Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItems(_testUser).Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(toDoItem.ItemId, result[0].ItemId);
            Assert.AreEqual(toDoItem.ItemTitle, result[0].ItemTitle);
            Assert.AreEqual(toDoItem.ItemStatus, result[0].ItemStatus);
            Assert.AreEqual(toDoItem.ItemDueOn, result[0].ItemDueOn);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldReturn_Null_When_NoToDoItemsByToDoStatusFound()
        {
            //set up
            var toDoItem = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItem.ItemId, toDoItem.ItemTitle, toDoItem.ItemStatus, toDoItem.ItemDueOn, toDoItem.UserId).Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItemsByStatus(_testUser, It.IsAny<ToDoStatuses>()).Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldReturn_AllToDoItems_When_ToDoItemsByToDoStatusFound()
        {
            //set up
            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .WithToDoItem(toDoItemTwo.ItemId, toDoItemTwo.ItemTitle, toDoItemTwo.ItemStatus, toDoItemTwo.ItemDueOn, toDoItemTwo.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItemsByStatus(_testUser, ToDoStatuses.InProgress).Result;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldReturn_Null_When_NoToDoItemsByItemIdFound()
        {
            //set up
            var toDoItem = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItem.ItemId, toDoItem.ItemTitle, toDoItem.ItemStatus, toDoItem.ItemDueOn, toDoItem.UserId).Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItemByItemId(_testUser, It.IsAny<int>()).Result;

            // assert
            Assert.IsNull(result);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldReturn_ToDoItem_When_ToDoItemByItemIdFound()
        {
            //set up
            int itemIdToFindBy = 1;

            var toDoItem = new ToDoItem
            {
                ItemId = itemIdToFindBy,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItem.ItemId, toDoItem.ItemTitle, toDoItem.ItemStatus, toDoItem.ItemDueOn, toDoItem.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.GetItemByItemId(_testUser, itemIdToFindBy).Result;

            // assert
            Assert.IsNotNull(result);

            Assert.AreEqual(toDoItem.ItemId, result.ItemId);
            Assert.AreEqual(toDoItem.ItemTitle, result.ItemTitle);
            Assert.AreEqual(toDoItem.ItemStatus, result.ItemStatus);
            Assert.AreEqual(toDoItem.ItemDueOn, result.ItemDueOn);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldCreate_ToDoItem_When_ToDoItemIsValid()
        {
            //set up
            var toDoItemToCreate = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite().Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var toDoItem = this._toDoService.CreateItem(_testUser, toDoItemToCreate).Result;

            // assert
            Assert.IsNotNull(toDoItem);
            Assert.AreEqual(toDoItem.ItemId, toDoItem.ItemId);
            Assert.AreEqual(toDoItem.ItemTitle, toDoItem.ItemTitle);
            Assert.AreEqual(toDoItem.ItemStatus, toDoItem.ItemStatus);
            Assert.AreEqual(toDoItem.ItemDueOn, toDoItem.ItemDueOn);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ShouldReturn_Null_When_ToDoItemIsNotFoundForUpdate()
        {
            // set up
            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.UpdateItem(_testUser, toDoItemTwo).Result;

            // assert
            Assert.IsNull(result);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldUpdate_ToDoItem_When_ToDoItemIsFound()
        {
            // set up
            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemToUpdate = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One For Update",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddDays(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .WithToDoItem(toDoItemTwo.ItemId, toDoItemTwo.ItemTitle, toDoItemTwo.ItemStatus, toDoItemTwo.ItemDueOn, toDoItemTwo.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var toDoItem = this._toDoService.UpdateItem(_testUser, toDoItemToUpdate).Result;

            // assert
            Assert.IsNotNull(toDoItem);
            Assert.AreNotEqual(toDoItemToUpdate, toDoItem);

            Assert.AreEqual(toDoItemToUpdate.ItemTitle, toDoItem.ItemTitle);
            Assert.AreEqual(toDoItemToUpdate.ItemStatus, toDoItem.ItemStatus);
            Assert.AreEqual(toDoItemToUpdate.ItemDueOn, toDoItem.ItemDueOn);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ShouldReturn_Null_When_ToDoItemIsNotFoundForStatusPatch()
        {
            // set up
            ToDoStatuses itemStatusToPatch = ToDoStatuses.InProgress;

            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var result = this._toDoService.PatchItemStatus(_testUser, toDoItemTwo.ItemId, itemStatusToPatch).Result;

            // assert
            Assert.IsNull(result);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldPatch_ToDoItemStatus_When_ToDoItemIsFoundForStatusPatch()
        {
            // set up
            ToDoStatuses itemStatusToPatch = ToDoStatuses.InProgress;

            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            var toDoItem = this._toDoService.PatchItemStatus(_testUser, toDoItemOne.ItemId, itemStatusToPatch).Result;

            // assert
            Assert.IsNotNull(toDoItem);

            Assert.AreEqual(itemStatusToPatch, toDoItem.ItemStatus);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ShouldReturn_Null_When_ToDoItemIsNotFoundForDelete()
        {
            // set up
            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            this._toDoService.DeleteItem(_testUser, toDoItemTwo.ItemId);

            // assert
            Assert.AreEqual(1, this._toDoService.GetItems(_testUser).Result.Count);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShouldDelete_ToDoItem_When_ToDoItemIsFoundForDelete()
        {
            // set up
            var toDoItemOne = new ToDoItem
            {
                ItemId = 1,
                ItemTitle = "Test To Do Item One",
                ItemStatus = ToDoStatuses.ToDo,
                ItemDueOn = DateTimeOffset.Now.AddHours(1),
                UserId = Guid.Parse(_testUser.Id)
            };

            var toDoItemTwo = new ToDoItem
            {
                ItemId = 2,
                ItemTitle = "Test To Do Item Two",
                ItemStatus = ToDoStatuses.InProgress,
                ItemDueOn = DateTimeOffset.Now.AddHours(2),
                UserId = Guid.Parse(_testUser.Id)
            };

            var mockDatabaseContext = new ToDoDbContextBuilder().UseInMemorySqlite()
                .WithToDoItem(toDoItemOne.ItemId, toDoItemOne.ItemTitle, toDoItemOne.ItemStatus, toDoItemOne.ItemDueOn, toDoItemOne.UserId)
                .WithToDoItem(toDoItemTwo.ItemId, toDoItemTwo.ItemTitle, toDoItemTwo.ItemStatus, toDoItemTwo.ItemDueOn, toDoItemTwo.UserId)
                .Build();

            var messageSender = new Mock<IMessageSender>();
            new Mock<IMessageSender>().Setup(i => i.SendMessage(It.IsAny<string>())).Returns(It.IsAny<Task>());

            this._toDoService = new ToDoService(mockDatabaseContext, this._eventStoreService, messageSender.Object);

            // test
            this._toDoService.DeleteItem(_testUser, toDoItemOne.ItemId);

            // assert
            var toDoItems = this._toDoService.GetItems(_testUser).Result;

            Assert.AreEqual(1, toDoItems.Count);
            Assert.AreEqual(toDoItemTwo.ItemId, toDoItems[0].ItemId);
            Assert.AreEqual(toDoItemTwo.ItemTitle, toDoItems[0].ItemTitle);
            Assert.AreEqual(toDoItemTwo.ItemStatus, toDoItems[0].ItemStatus);
            Assert.AreEqual(toDoItemTwo.ItemDueOn, toDoItems[0].ItemDueOn);

            // messageSender.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
        }
    }
}