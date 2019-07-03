using System;
using ToDo.Persistent.DbEnums;

namespace ToDo.Persistent.DbObjects
{
    public class ToDoItem
    {
        public int ItemId { get; set; }
        
        public string ItemTitle { get; set; }

        public ToDoStatuses ItemStatus { get; set; }

        public int UserId { get; set; }

        public DateTimeOffset? ItemDueDateTime { get; set; }
    }
}