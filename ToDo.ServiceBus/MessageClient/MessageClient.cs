using Microsoft.Azure.ServiceBus;

namespace ToDo.ServiceBus.MessageClient
{
    public class MessageClient : IMessageClient
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _queueName;

        public MessageClient(string serviceBusConnectionString, string queueName)
        {
            this._serviceBusConnectionString = serviceBusConnectionString;
            this._queueName = queueName;
        }

        public IQueueClient GetMessageClient()
        {
            return new QueueClient(this._serviceBusConnectionString, this._queueName);
        }
    }
}