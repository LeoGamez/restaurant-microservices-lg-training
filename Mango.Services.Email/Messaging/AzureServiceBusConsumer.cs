using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.Email.Models;
using Mango.Services.Email.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IEmailRepository emailRepository;
        private readonly IConfiguration configuration;

        private readonly string serviceBusConnectionString;
        private readonly string emailSubscription;
        private readonly string updatePaymentTopic;

        private ServiceBusProcessor processor;

        public AzureServiceBusConsumer(IEmailRepository emailRepository, IConfiguration configuration)
        {
            this.emailRepository = emailRepository;
            this.configuration = configuration;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            emailSubscription = configuration.GetValue<string>("SubscriptionName");
            updatePaymentTopic = configuration.GetValue<string>("UpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            processor = client.CreateProcessor(updatePaymentTopic, emailSubscription);

        }

        public async Task Start()
        {
            processor.ProcessMessageAsync += OnPaymentReceieved;
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

        public async Task OnPaymentReceieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body=Encoding.UTF8.GetString(message.Body);

                var messageBody = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

                if (messageBody != null)
                {
                    try
                    {
                        await emailRepository.SendAndLogEmail(messageBody);
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
}
