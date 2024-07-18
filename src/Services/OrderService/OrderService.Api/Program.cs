using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Shared.Events.Order;
using EventBus.Shared.Events.Payment;
using EventBus.Shared.Events.Stock;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Api.IntegrationEventHandlers;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Extensions;
using OrderService.Infrastructure.Persistence.Context;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order API",
        Version = "v1",
        Description = "API for getting order details",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    //// Include XML comments for Swagger documentation
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

var config = EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name);
ConfigureEventBusServices(builder.Services, config);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1"); });
}


app.UseHttpsRedirection();

app.MapControllers();

#if DEBUG
app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContext>>();

    var dbContextSeeder = new OrderDbContextSeed();
    dbContextSeeder.SeedAsync(context, logger).Wait();
});

#endif
if (config.EventBusType != EventBusType.MassTransit_RabbitMQ)
    ConfigureEventBusForSubscription(app);

app.Run();

void ConfigureEventBusServices(IServiceCollection services, EventBusConfig config)
{
    services.AddSingleton(sp => EventBusFactory.Create(config, sp)); //OrderService
    if (config.EventBusType == EventBusType.MassTransit_RabbitMQ)
    {
        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<OrderPreparedIntegrationEventHandler>();
            configurator.AddConsumer<PaymentCompletedIntegrationEventHandler>();
            configurator.AddConsumer<PaymentFailedIntegrationEventHandler>();
            configurator.AddConsumer<StockNotReservedIntegrationEventHandler>();
            configurator.UsingRabbitMq((context, _configure) =>
            {
                _configure.Host(builder.Configuration["RabbitMQ"]);

                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<OrderPreparedIntegrationEventHandler>(context));
                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<PaymentCompletedIntegrationEventHandler>(context));
                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<PaymentFailedIntegrationEventHandler>(context));
                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<StockNotReservedIntegrationEventHandler>(context));
            });
        });
    }
    services
        .AddLogging(configure => configure.AddConsole())
        .AddApplicationRegistration()
        .AddPersistenceRegistration(builder.Configuration)
        .ConfigureEventHandlers()
        .AddServiceDiscoveryRegistration(builder.Configuration);  
}


void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderPreparedIntegrationEvent, OrderPreparedIntegrationEventHandler>();
    eventBus.Subscribe<PaymentCompletedIntegrationEvent, PaymentCompletedIntegrationEventHandler>();
    eventBus.Subscribe<PaymentFailedIntegrationEvent, PaymentFailedIntegrationEventHandler>();
    eventBus.Subscribe<StockNotReservedIntegrationEvent, StockNotReservedIntegrationEventHandler>();
}
