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

        public async Task<BrokeredMessage> ProcessMessage(string messageBody)
        {
            // send message
            await this.SendMessages(messageBody);
            
            // receive message
            return await this.ReceiveMessages();
        }

        private async Task SendMessages(string messageBody)
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

        private async Task<BrokeredMessage> ReceiveMessages()
        {
            try
            {
                return await this._queueClient.QueueClient.ReceiveAsync(TimeSpan.FromSeconds(30));
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