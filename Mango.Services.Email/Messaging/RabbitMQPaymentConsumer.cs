
using Mango.Services.Email.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IEmailRepository emailRepository;


        private IConnection connection;
        private IModel channel;
        private string queueName = "";


        public RabbitMQPaymentConsumer(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "updatePaymenExchange",ExchangeType.Fanout,false,false);
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "updatePaymenExchange", "");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                if (updatePaymentResultMessage != null)
                {
                    HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();

                }
                channel.BasicAck(ea.DeliveryTag, false);

            };

            channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {

            try
            {
                await emailRepository.SendAndLogEmail(updatePaymentResultMessage);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
