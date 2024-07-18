using BasketService.Api.Extensions;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Shared.Events.Order;
using EventBus.Shared.Events.Payment;
using MassTransit;
using StockService.Api.Extensions;
using StockService.Api.IntegrationEventHandlers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name);
ConfigureEventBusServices(builder.Services, config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.RegisterWithConsul(lifetime);

if (config.EventBusType != EventBusType.MassTransit_RabbitMQ)
    ConfigureEventBusForSubscription(app);

app.Run();

void ConfigureEventBusServices(IServiceCollection services, EventBusConfig config)
{
    services.AddSingleton(sp => EventBusFactory.Create(config, sp)); //BasketService

    if (config.EventBusType == EventBusType.MassTransit_RabbitMQ)
    {
        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<PaymentFailedIntegrationEventHandler>();
            configurator.AddConsumer<OrderCreatedIntegrationEventHandler>();
            configurator.UsingRabbitMq((context, _configure) =>
            {
                _configure.Host(builder.Configuration["RabbitMQ"]);

                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<PaymentFailedIntegrationEventHandler>(context));
                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<OrderCreatedIntegrationEventHandler>(context));
            });
        });
    }

    builder.Services.AddTransient<PaymentFailedIntegrationEventHandler>();
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    eventBus.Subscribe<PaymentFailedIntegrationEvent, PaymentFailedIntegrationEventHandler>();
    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}
