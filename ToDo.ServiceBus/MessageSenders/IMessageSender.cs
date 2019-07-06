using System.Threading.Tasks;

namespace ToDo.ServiceBus.MessageSenders
{
    public interface IMessageSender
    {
        Task SendMessage(string messageBody);
    }
}