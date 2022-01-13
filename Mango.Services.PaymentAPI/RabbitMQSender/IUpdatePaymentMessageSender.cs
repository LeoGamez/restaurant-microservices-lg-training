using Mango.MessageBus;

namespace Mango.Services.PaymentApi.RabbitMQSender
{
    public interface IUpdatePaymentMessageSender
    {
        void SendMessage(BaseMessage baseMessage,String queueName);
    }
}
