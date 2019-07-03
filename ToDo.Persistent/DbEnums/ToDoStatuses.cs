using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ToDo.Domain.Attributes;

namespace ToDo.Persistent.DbEnums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ToDoStatuses
    {
        NotSpecified = 0,

        [Display(Name = "ToDo", Description = "The task has been created, but it is not started.")]
        [StringValue("ToDo")]
        ToDo = 1,

        [Display(Name = "InProgress", Description = "The task has been in progress, but it is not completed.")]
        [StringValue("InProgress")]
        InProgress = 2,

        [Display(Name = "Completed", Description = "The task has been completed.")]
        [StringValue("Completed")]
        Completed = 3,
    }
}