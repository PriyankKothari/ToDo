using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ToDo.ServiceBus.MessageSenders
{
    public class MessageSender : IMessageSender
    {
        private readonly IQueueClient _queueClient;

        public MessageSender(string serviceBusConnectionString, string queueName)
        {
            this._queueClient = new QueueClient(serviceBusConnectionString, queueName);
        }

        public async Task SendMessage(string messageBody)
        {
            try
            {
                await this._queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(messageBody)));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}