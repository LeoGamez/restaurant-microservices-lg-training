using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrdersAPI.Models;
using Mango.Services.OrdersAPI.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrdersAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IOrderRepository orderRepository;
        private readonly IConfiguration configuration;
        private readonly IMessageBus messageBus;

        private readonly string serviceBusConnectionString;
        private readonly string checkOutSubscription;
        private readonly string checkOutMessageTopic;
        private readonly string paymentProcessTopic;
        private readonly string paymentProcessSubscription;
        private readonly string updatePaymentTopic;

        private ServiceBusProcessor processor;
        private ServiceBusProcessor paymentProcessor;

        public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            this.orderRepository = orderRepository;
            this.configuration = configuration;
            this.messageBus = messageBus;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            checkOutSubscription = configuration.GetValue<string>("SubscriptionName");
            checkOutMessageTopic = configuration.GetValue<string>("CheckoutMessageTopic");
            paymentProcessSubscription = configuration.GetValue<string>("mangoPaymentSubscription");
            paymentProcessTopic = configuration.GetValue<string>("PaymentProcessTopic");
            updatePaymentTopic = configuration.GetValue<string>("UpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            //processor = client.CreateProcessor(checkOutMessageTopic, checkOutSubscription);
            processor = client.CreateProcessor(checkOutMessageTopic);
            paymentProcessor = client.CreateProcessor(updatePaymentTopic, checkOutSubscription);

        }

        public async Task Start()
        {
            processor.ProcessMessageAsync += OnCheckOutMessageReceived;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();

            paymentProcessor.ProcessMessageAsync += OnPaymentReceieved;
            paymentProcessor.ProcessErrorAsync += ErrorHandler;
            await paymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();

            await paymentProcessor.StopProcessingAsync();
            await paymentProcessor.DisposeAsync();
        }

        public Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body=Encoding.UTF8.GetString(message.Body);

                CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

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
                    OrderDateTime= DateTime.Now,
                    //OrderHeaderId= checkoutHeaderDto.CartHeaderId,
                    PickupDateTime= checkoutHeaderDto.PickupDateTime,
                    PaymentStatus=false,
                    OrderTotal= checkoutHeaderDto.OrderTotal,   
                    OrderWithDiscountTotal = checkoutHeaderDto.OrderWithDiscountTotal,
                    PhoneNumber= checkoutHeaderDto.PhoneNumber, 
                    CartTotalItems= checkoutHeaderDto.CartTotalItems
                };

                foreach(var detail in checkoutHeaderDto.CartDetails)
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

                var result= await orderRepository.AddOrder(orderHeader);

                PaymentRequestMessage paymentRequestMessage = new()
                {
                    Name =orderHeader.FirstName+ " "+ orderHeader.LastName,
                    CardNumber = orderHeader.CardNumber,
                    CVV = orderHeader.CVV,
                    ExpiryMonthYear= orderHeader.ExpiryMonthYear,
                    OrderId = orderHeader.OrderHeaderId,
                    OrderTotal = orderHeader.OrderTotal,
                    Email = orderHeader.Email,
                };

                try
                {
                    await messageBus.PublishMessage(paymentRequestMessage, paymentProcessTopic);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task OnPaymentReceieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);

                var paymentREsultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                await orderRepository.UpdateOrderPaymentStatus(paymentREsultMessage.OrderId, paymentREsultMessage.Status);
                await args.CompleteMessageAsync(args.Message);
            }
        }
    }
}
