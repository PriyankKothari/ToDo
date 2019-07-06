using System;
using Newtonsoft.Json;

namespace ToDo.ConsoleApp
{
    class Program
    {
        public static Event CreateItemEvent = new Event
        {
            EventId = 1,
            EventType = "EventType",
            AggregateId = 1,
            AggregateName = "Aggregate Name",
            EventCreateDateTime = new DateTime(2019, 07, 05),
            EventPayLoad = string.Empty
        };

        static void Main(string[] args)
        {
            Console.WriteLine(CreateItemEvent.ObjectToJsonOject());
            Console.ReadLine();
            var @event = CreateItemEvent.ObjectToJsonOject().JsonObjectToObject<Event>();
            Console.WriteLine(@event.EventId);
            Console.WriteLine(@event.EventType);
            Console.WriteLine(@event.AggregateId);
            Console.WriteLine(@event.AggregateName);
            Console.WriteLine(@event.EventCreateDateTime);
            Console.WriteLine(@event.EventPayLoad);
            Console.ReadLine();
        }
    }

    public static class EventHelper
    {
        public static string ObjectToJsonOject<T>(this T objectToCovert) where T : class
        {
            return JsonConvert.SerializeObject(objectToCovert);
        }

        public static T JsonObjectToObject<T>(this string jsonObject) where T : class
        {
            return JsonConvert.DeserializeObject<T>(jsonObject);
        }
    }

    public class Event
    {
        public int EventId { get; set; }

        public string EventType { get; set; }

        public int AggregateId { get; set; }

        public string AggregateName { get; set; }

        public DateTime EventCreateDateTime { get; set; }

        public string EventPayLoad { get; set; }
    }
}
