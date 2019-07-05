using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ToDo.ServiceBus.MessageProcessors
{
    public interface IMessageProcessor
    {
        Task<BrokeredMessage> ProcessMessage(string messageBody);
    }
}