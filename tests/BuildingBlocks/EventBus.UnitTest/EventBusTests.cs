using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using System.Reflection;

namespace EventBus.UnitTest;

public class EventBusTests
{
    private ServiceCollection services;

    public EventBusTests()
    {
        services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
    }

    [Fact]
    public void Subscribe_Event_On_RabbitMQ()
    {
       services.AddSingleton(sp =>
           EventBusFactory.Create(
               EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name), 
               sp));

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [Fact]
    public void Subscribe_Event_On_AzureSB()
    {      
        services.AddSingleton(sp =>
          EventBusFactory.Create(
              EventBusConfig.GetAzureSBConfig(Assembly.GetExecutingAssembly().GetName().Name), 
              sp));

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        Task.Delay(2000).Wait();
    }

    [Fact]
    public void Send_Message_To_RabbitMQ()
    {
        services.AddSingleton(sp =>
           EventBusFactory.Create(
               EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name), 
               sp));

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    [Fact]
    public void Send_Message_To_AzureSB()
    {      
        services.AddSingleton<IEventBus>(sp =>
         EventBusFactory.Create(
             EventBusConfig.GetAzureSBConfig(Assembly.GetExecutingAssembly().GetName().Name),
             sp));

        var serviceProvider = services.BuildServiceProvider();

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

   
}