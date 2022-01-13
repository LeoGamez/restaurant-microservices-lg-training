
using Mango.Services.PaymentApi.Messages;
using Mango.Services.PaymentApi.RabbitMQSender;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.PaymentApi.Messaging
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly IProcessPayment processPayment;
        private readonly IUpdatePaymentMessageSender updatePaymentMessageSender;


        private IConnection connection;
        private IModel channel;

      
        public RabbitMQCheckoutConsumer(IProcessPayment processPayment, IUpdatePaymentMessageSender updatePaymentMessageSender)
        {
            this.processPayment = processPayment;
            this.updatePaymentMessageSender = updatePaymentMessageSender;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);

                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();

                channel.BasicAck(ea.DeliveryTag, false);

            };

            channel.BasicConsume("orderpaymentprocesstopic", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            var result = processPayment.PaymentProcess();

            var updatePaymentMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };

            try
            {
                updatePaymentMessageSender.SendMessage(updatePaymentMessage, "updatePaymenExchange");

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
