using EventBus.Base;
using EventBus.Base.Events;
using MassTransit;
using Newtonsoft.Json;
using System.Text;

namespace EventBus.MassTransit.RabbitMQ
{
    public class EventBusMassTransitRabbitMQ : BaseEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public EventBusMassTransitRabbitMQ(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, IServiceProvider serviceProvider, EventBusConfig eventBusConfig) 
            : this(serviceProvider, eventBusConfig)
        {
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public EventBusMassTransitRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) 
            : base(serviceProvider, eventBusConfig) {}

      
        public override async Task Publish(IntegrationEvent @event)
        {
            //var eventName = @event.GetType().Name;
            //eventName = TrimEventName(eventName);

            //var message = JsonConvert.SerializeObject(@event);
            //var body = Encoding.UTF8.GetBytes(message);

            await _publishEndpoint.Publish(@event);
        }

        public override async Task SendEndPoint(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            eventName = TrimEventName(eventName);
            var sendEndpoint = _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{GetQueueName(eventName)}")).GetAwaiter().GetResult();
            await sendEndpoint.Send(@event);
        }

        public override void Subscribe<T, TH>()
        {
          
        }

        public override void UnSubscribe<T, TH>()
        {
            
        }

      
    }
}
