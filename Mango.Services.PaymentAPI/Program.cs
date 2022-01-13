using Mango.MessageBus;
using Mango.Services.PaymentApi.Extensions;
using Mango.Services.PaymentApi.Messaging;
using Mango.Services.PaymentApi.RabbitMQSender;
using PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddSingleton<IMessageBus,AzureServiceMessageBus>();
builder.Services.AddSingleton<IUpdatePaymentMessageSender, UpdatePaymentMessageSender>();

builder.Services.AddHostedService<RabbitMQCheckoutConsumer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.UseAzureServiceBusConstumer();

app.MapControllers();

app.Run();