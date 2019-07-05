using Microsoft.ServiceBus.Messaging;

namespace ToDo.ServiceBus.MessageClient
{
    public class MessageClient : IMessageClient
    {
        public QueueClient QueueClient { get; set; }

        public MessageClient(string serviceBusConnectionString, string queueName)
        {
            this.QueueClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, queueName);
        }
    }
}