using System.Threading.Tasks;

namespace ToDo.ServiceBus.MessageProcessors
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(string messageBody);
    }
}