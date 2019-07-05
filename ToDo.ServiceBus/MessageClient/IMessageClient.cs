using Microsoft.Azure.ServiceBus;

namespace ToDo.ServiceBus.MessageClient
{
    public interface IMessageClient
    {
        IQueueClient GetMessageClient();
    }
}