using Mango.MessageBus;

namespace Mango.Services.OrdersAPI.RabbitMQSender
{
    public interface IOrderMessageSender
    {
        void SendMessage(BaseMessage baseMessage,String queueName);
    }
}
