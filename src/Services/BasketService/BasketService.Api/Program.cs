using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEventHandlers;
using BasketService.Api.IntegrationEvents;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Shared.Events.Payment;
using EventBus.Shared.Events.Stock;
using MassTransit;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.SetEnvironment();

// Setting Serilog
builder.AddSerilogConfiguration();


// Add services to the container.
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

var config = EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name);
ConfigureEventBusServices(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
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
            configurator.AddConsumer<StockNotReservedIntegrationEventHandler>();
            configurator.UsingRabbitMq((context, _configure) =>
            {
                _configure.Host(builder.Configuration["RabbitMQ"]);

                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<PaymentFailedIntegrationEventHandler>(context));
                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<StockNotReservedIntegrationEventHandler>(context));
            });
        });
    }
  
    builder.Services.AddTransient<PaymentFailedIntegrationEventHandler>();
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    eventBus.Subscribe<PaymentFailedIntegrationEvent, PaymentFailedIntegrationEventHandler>();
    eventBus.Subscribe<StockNotReservedIntegrationEvent, StockNotReservedIntegrationEventHandler>();
}