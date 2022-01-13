using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.OrdersAPI.RabbitMQSender
{
    public class OrderMessageSender : IOrderMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;

        private IConnection connection;

        public OrderMessageSender()
        {
            _hostname = "localhost";
            _password = "guest";
            _username = "guest";
        }

        public void SendMessage(BaseMessage message, string queueName)
        {

            if (ConnectionExists())
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                connection = factory.CreateConnection();

                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body); 
            }


        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                connection = factory.CreateConnection();

            }
            catch (Exception ex)
            {

            }
        }

        private bool ConnectionExists()
        {
            if(connection == null)
            {
                CreateConnection();
                return connection != null;
            }
            else
            {
                return true;

            }
        }
    }
}
