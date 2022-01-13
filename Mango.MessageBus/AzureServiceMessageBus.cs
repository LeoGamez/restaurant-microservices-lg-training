using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureServiceMessageBus : IMessageBus
    {
        //todo:  To Fix
        private string connectionString = "Endpoint=sb://mangorestaurant-lg2022.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bILepqodgVEa0kAt8htCiE3ZEewl1TayQBRel+WWUss=";

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            var sender= client.CreateSender(topicName);

            var messageJson= JsonConvert.SerializeObject(message);


            var busMessage= new ServiceBusMessage(Encoding.UTF8.GetBytes(messageJson))
            {
                CorrelationId= Guid.NewGuid().ToString()
            };
            
            await  sender.SendMessageAsync(busMessage);

            await sender.CloseAsync();
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
