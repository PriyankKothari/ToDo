using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
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

        public async Task ProcessMessage(string messageBody, Func<Message, CancellationToken, Task> processMessage)
        {
            // send message
            await this._queueClient.GetMessageClient().SendAsync(new Message(Encoding.UTF8.GetBytes(messageBody)));

            // receive & process message
            this._queueClient.GetMessageClient().RegisterMessageHandler(processMessage,
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 1, AutoComplete = false });
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}