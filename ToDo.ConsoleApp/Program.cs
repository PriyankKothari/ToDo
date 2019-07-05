using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ToDo.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
            GetMessage();
        }

        static async Task MainAsync()
        {
            const string ServiceBusConnectionString = "Endpoint=sb://todoevents.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9dQF7WE0Mf9tqe0CO+kMjO3TSPWiSbjX+OuAWoBYs+k=";
            const string QueueName = "todoeventsqueue";
            IQueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            string messageBody = $"Message One - Test Message";
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);
        }

        static void GetMessage()
        {
            Console.ReadLine();
        }
    }
}