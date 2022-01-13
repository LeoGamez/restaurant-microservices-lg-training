using Mango.Services.OrdersAPI.Models;
using Mango.Services.OrdersAPI.RabbitMQSender;
using Mango.Services.OrdersAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.OrdersAPI.Messaging
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderMessageSender orderMessageSender;
        private IConnection connection;
        private IModel channel;

      
        public RabbitMQCheckoutConsumer(IOrderRepository orderRepository, IOrderMessageSender orderMessageSender)
        {
            this.orderRepository = orderRepository;
            this.orderMessageSender = orderMessageSender;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);

                HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();

                channel.BasicAck(ea.DeliveryTag, false);

            };

            channel.BasicConsume("checkoutqueue",false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
        {
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderDateTime = DateTime.Now,
                //OrderHeaderId= checkoutHeaderDto.CartHeaderId,
                PickupDateTime = checkoutHeaderDto.PickupDateTime,
                PaymentStatus = false,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                OrderWithDiscountTotal = checkoutHeaderDto.OrderWithDiscountTotal,
                PhoneNumber = checkoutHeaderDto.PhoneNumber,
                CartTotalItems = checkoutHeaderDto.CartTotalItems
            };

            foreach (var detail in checkoutHeaderDto.CartDetails)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.Product.Name,
                    Price = detail.Product.Price,
                    Count = detail.Count
                };

                orderHeader.CartTotalItems += detail.Count;
                orderHeader.OrderDetails.Add(orderDetail);
            }

            var result = await orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email,
            };

            try
            {
                orderMessageSender.SendMessage(paymentRequestMessage, "orderpaymentprocesstopic");

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
