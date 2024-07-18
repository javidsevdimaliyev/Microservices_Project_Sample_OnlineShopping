using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.Shared.Events.Payment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Console.IntegrationEvents;
using System.Reflection;

ServiceCollection services = new ServiceCollection();

ConfigureServices(services);

var sp = services.BuildServiceProvider();

IEventBus eventBus = sp.GetRequiredService<IEventBus>();
eventBus.Subscribe<PaymentFailedIntegrationEvent, PaymentFailedIntegrationEventHandler>();
eventBus.Subscribe<PaymentCompletedIntegrationEvent, PaymentCompletedIntegrationEventHandler>();

Console.WriteLine("Notification Service App and Running...");
Console.ReadLine();

void ConfigureServices(ServiceCollection services)
{
    var config = EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name);
    services.AddSingleton(sp => EventBusFactory.Create(config, sp)); //NotificationService 

    services.AddLogging(configure =>
    {
        configure.AddConsole();
    });

    services.AddTransient<PaymentFailedIntegrationEventHandler>();
    services.AddTransient<PaymentCompletedIntegrationEventHandler>();  
}
