using System;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToDo.Persistent.DbEnums;
using ToDo.Validators;

namespace ToDo.Tests.Validators
{
    [TestClass]
    public class ToDoItemValidatorTests
    {
        private readonly ToDoItemValidator _toDoItemValidator;

        private readonly string _moreThan100Characters = "pneumonoultramicroscopicsilicovolcanoconiosisparastratiosphecomyiastratiosphecomyioidesfloccinaucinition"; // 104 characters

        public ToDoItemValidatorTests()
        {
            this._toDoItemValidator = new ToDoItemValidator();
        }

        [TestMethod]
        public void ValidateToDoItemTitle_Empty_Failed()
        {
            this._toDoItemValidator.ShouldHaveValidationErrorFor(i => i.ItemTitle, It.IsAny<string>())
                .WithErrorMessage("ToDo Item Title should not be empty");
        }

        [TestMethod]
        public void ValidateToDoItemTitle_MoreThan100Characters_Failed()
        {
            // assert
            this._toDoItemValidator.ShouldHaveValidationErrorFor(i => i.ItemTitle, _moreThan100Characters)
                .WithErrorMessage("ToDo Item Title should be no more than 100 characters");
        }

        [TestMethod]
        public void ValidateToDoItemTitle_NotEmpty_LessThan100Characters_Passed()
        {
            // assert
            this._toDoItemValidator.ShouldNotHaveValidationErrorFor(i => i.ItemTitle,
                "Test To Do Item");
        }

        [TestMethod]
        public void ValidateToDoItemStatus_Empty_Failed()
        {
            // assert
            this._toDoItemValidator.ShouldHaveValidationErrorFor(i => i.ItemStatus, It.IsAny<ToDoStatuses>())
                .WithErrorMessage("ToDo Item Status should not be empty");
        }

        [TestMethod]
        public void ValidateToDoItemStatus_NotInEnum_Failed()
        {
            // assert
            this._toDoItemValidator.ShouldHaveValidationErrorFor(i => i.ItemStatus, (ToDoStatuses)10)
                .WithErrorMessage("Please provide valid ToDo item status");
        }

        [TestMethod]
        public void ValidateToDoItemStatus_NotEmpty_InEnum_Passed()
        {
            // assert
            this._toDoItemValidator.ShouldNotHaveValidationErrorFor(i => i.ItemStatus, ToDoStatuses.InProgress);
        }

        [TestMethod]
        public void ValidateToDoItemDueOn_InPast_Failed()
        {
            // assert
            this._toDoItemValidator.ShouldHaveValidationErrorFor(i => i.ItemDueOn, DateTime.Now.AddHours(-1))
                .WithErrorMessage("ToDo Item Due Date must be in future");
        }

        [TestMethod]
        public void ValidateInvoiceDate_NonEmpty_InFuture_Passed()
        {
            // assert
            this._toDoItemValidator.ShouldNotHaveValidationErrorFor(i => i.ItemDueOn, DateTime.Now.AddHours(1));
        }
    }
}