using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ToDo.ServiceBus.MessageProcessors
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(string messageBody, Func<Message, CancellationToken, Task> processMessage);
    }
}