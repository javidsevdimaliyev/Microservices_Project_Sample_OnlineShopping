using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Shared.Events.Stock;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PaymentService.Api.IntegrationEventHandlers;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.AddDebug();
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

app.UseHttpsRedirection();

app.MapControllers();


if (config.EventBusType != EventBusType.MassTransit_RabbitMQ)
    ConfigureEventBusForSubscription(app);

app.Run();

void ConfigureEventBusServices(IServiceCollection services, EventBusConfig config)
{    
    services.AddSingleton(sp => EventBusFactory.Create(config, sp)); //PaymentService
    if (config.EventBusType == EventBusType.MassTransit_RabbitMQ)
    {
        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<StockReservedIntegrationEventHandler>();
            configurator.UsingRabbitMq((context, _configure) =>
            {
                _configure.Host(builder.Configuration["RabbitMQ"]);

                _configure.ReceiveEndpoint(config.SubscriberClientAppName, e => e.ConfigureConsumer<StockReservedIntegrationEventHandler>(context));
            });
        });
    }
    builder.Services.AddTransient<StockReservedIntegrationEventHandler>();
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    eventBus.Subscribe<StockReservedIntegrationEvent, StockReservedIntegrationEventHandler>();
}


