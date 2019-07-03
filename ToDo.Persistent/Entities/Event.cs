using System;

namespace ToDo.Persistent.Entities
{
    public class Events
    {
        public int EventId { get; set; }

        public string EventType { get; set; }

        public int AggregateId { get; set; }

        public string AggregateName { get; set; }

        public DateTime EventCreateDateTime { get; set; }

        public string EventPayLoad { get; set; }
    }
}