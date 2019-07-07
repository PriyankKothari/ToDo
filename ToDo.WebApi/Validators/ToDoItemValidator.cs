using System;
using FluentValidation;
using ToDo.Persistent.DbEnums;
using ToDo.Persistent.DbObjects;

namespace ToDo.WebApi.Validators
{
    /// <summary>
    /// To-Do item validations
    /// </summary>
    public class ToDoItemValidator : AbstractValidator<ToDoItem>
    {
        /// <summary>
        /// Setting all the validation rules
        /// </summary>
        public ToDoItemValidator()
        {
            // validate item title is not empty and maximum length is 500 characters
            RuleFor(td => td.ItemTitle)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("ToDo Item Title should not be empty")
                .MaximumLength(100).WithMessage("ToDo Item Title should be no more than 100 characters");

            // validate item status is provided and is a valid enum value
            RuleFor(td => td.ItemStatus)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEqual(ToDoStatuses.NotSpecified).WithMessage("ToDo Item Status should not be empty")
                .IsInEnum().WithMessage("Please provide valid ToDo item status");

            // validate item due date is not empty and is greater then now
            RuleFor(td => td.ItemDueOn)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("ToDo Item Due Date should not be empty")
                .GreaterThan(DateTimeOffset.Now).WithMessage("ToDo Item Due Date must be in future");
        }
    }
}