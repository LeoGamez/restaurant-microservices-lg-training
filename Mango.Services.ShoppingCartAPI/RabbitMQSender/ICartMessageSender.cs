using Mango.MessageBus;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface ICartMessageSender
    {
        void SendMessage(BaseMessage baseMessage,String queueName);
    }
}
