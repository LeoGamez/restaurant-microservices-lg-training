using Mango.Services.PaymentApi.Messaging;

namespace Mango.Services.PaymentApi.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IAzureServiceBusConsumer ServiceBusConsumter { get; set; }

        public static WebApplication UseAzureServiceBusConstumer (this WebApplication app)
        {
            ServiceBusConsumter =app.Services.GetService<IAzureServiceBusConsumer>();
            var hosApplicationLife = app.Services.GetService<IHostApplicationLifetime>();

            hosApplicationLife.ApplicationStarted.Register(OnStart);
            hosApplicationLife.ApplicationStopped.Register(OnStop);

            return app;

        }

        private static void OnStop()
        {
            ServiceBusConsumter.Stop();
        }

        private static void OnStart()
        {
            ServiceBusConsumter.Start();
        }
    }
}
