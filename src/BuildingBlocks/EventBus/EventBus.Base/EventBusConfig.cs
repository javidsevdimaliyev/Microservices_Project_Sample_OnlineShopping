namespace EventBus.Base;

public class EventBusConfig
{
    public int ConnectionRetryCount { get; set; } = 5;

    public string DefaultTopicName { get; set; } = "OnlineShoppingExchange";

    public string EventBusConnectionString { get; set; } = String.Empty;

    public string SubscriberClientAppName { get; set; } = String.Empty;

    public string EventNamePrefix { get; set; } = String.Empty;

    public string EventNameSuffix { get; set; } = "IntegrationEvent";

    public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;

    public object Connection { get; set; }

    public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);

    public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);

    public static EventBusConfig GetRabbitMQConfig(string subscriberClientAppName)
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = subscriberClientAppName,
            DefaultTopicName = "OnlineShoppingRabbitMQTopic",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString =
                "amqps://qhvzutuk:nnEjWdeS6RFRj2bZG_3wqJYXHenmiccK@sparrow.rmq.cloudamqp.com/qhvzutuk"
            // Defined as default so that we do not need to specify again
            //Connection = new ConnectionFactory()
            //{
            //    HostName = "c_rabbitmq",
            //    Port = 5672,
            //    UserName = "guest",
            //    Password = "guest"
            //}
        };
    }

    public static EventBusConfig GetMasstransitRabbitMQConfig(string subscriberClientAppName)
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = subscriberClientAppName,
            DefaultTopicName = "OnlineShoppingMassTransitTopic",
            EventBusType = EventBusType.MassTransit_RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString =
                "amqps://qhvzutuk:nnEjWdeS6RFRj2bZG_3wqJYXHenmiccK@sparrow.rmq.cloudamqp.com/qhvzutuk"           
        };
    }

    public static EventBusConfig GetAzureSBConfig(string subscriberClientAppName)
    {
        return new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = subscriberClientAppName,
            DefaultTopicName = "OnlineShoppingTopicName",
            EventBusType = EventBusType.AzureServiceBus,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString = "Endpoint=sb://onlineshopping.servicebus.windows.net/:..."
        };
    }
}
