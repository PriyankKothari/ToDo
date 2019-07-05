using Microsoft.ServiceBus.Messaging;

namespace ToDo.ServiceBus.MessageClient
{
    public interface IMessageClient
    {
        QueueClient QueueClient { get; set; }
    }
}