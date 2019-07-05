using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using ToDo.ServiceBus.MessageClient;

namespace ToDo.ServiceBus.MessageProcessors
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageClient _queueClient;

        public MessageProcessor(IMessageClient queueClient)
        {
            this._queueClient = queueClient;
        }

        public async Task ProcessMessage(string messageBody)
        {
            try
            {
                await this._queueClient.QueueClient.SendAsync(new BrokeredMessage(messageBody));
            }
            catch (MessagingException exception)
            {
                throw exception.IsTransient
                    ? new Exception(exception.Message)
                    : new MessagingException(exception.Message);
            }
        }
    }
}