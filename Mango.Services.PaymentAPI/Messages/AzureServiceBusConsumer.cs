using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentApi.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly IProcessPayment processPayment;
        private readonly IMessageBus messageBus;

        private readonly string serviceBusConnectionString;
        private readonly string paymentProcessTopic;
        private readonly string updatePaymentTopic;
        private readonly string paymentProcessSubscription;
        private readonly string updatePaymentSubscription;

        private ServiceBusProcessor processor;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
        {
            this.processPayment = processPayment;
            this.configuration = configuration;
            this.messageBus = messageBus;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            paymentProcessSubscription = configuration.GetValue<string>("PaymentProcessSubscription");
            paymentProcessTopic = configuration.GetValue<string>("PaymentProcessTopic");
            updatePaymentTopic = configuration.GetValue<string>("UpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            processor = client.CreateProcessor(paymentProcessTopic, paymentProcessSubscription);            

        }

        public async Task Start()
        {
            processor.ProcessMessageAsync += OnPaymentMessageReceived;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }

        public Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnPaymentMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body=Encoding.UTF8.GetString(message.Body);

                var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

                var result = processPayment.PaymentProcess();

                var updatePaymentMessage = new UpdatePaymentResultMessage()
                {
                    Status = result,
                    OrderId = paymentRequestMessage.OrderId,
                    Email = paymentRequestMessage.Email
                };
                           

                try
                {
                    await messageBus.PublishMessage(updatePaymentMessage, updatePaymentTopic);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
