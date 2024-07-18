using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Order;
using EventBus.Shared.Events.Stock;
using MassTransit;
using MongoDB.Driver;
using StockService.Api.Infrastructure;

namespace StockService.Api.IntegrationEventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>, IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly MongoDBRepository _mongoDBService;
        private readonly IEventBus _eventBus;
        public OrderCreatedIntegrationEventHandler(MongoDBRepository mongoDBService, IEventBus eventBus)
        {
            _mongoDBService = mongoDBService;
            _eventBus = eventBus;
        }
        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            List<bool> stockResult = new();
            IMongoCollection<Infrastructure.Models.Stock> collection = _mongoDBService.GetCollection<Infrastructure.Models.Stock>();

            foreach (var orderItem in @event.OrderItems)
            {
                stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString() && s.Count > (long)orderItem.Quantity)).AnyAsync());
            }

            if (stockResult.TrueForAll(s => s.Equals(true)))
            {
                //Stock update...
                foreach (var orderItem in @event.OrderItems)
                {
                    Infrastructure.Models.Stock stock = await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                    stock.Count -= orderItem.Quantity;

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId.ToString(), stock);
                }
                //Throwing the event that will warn Payment...
                StockReservedIntegrationEvent stockReservedEvent = new()
                {
                    BuyerId = @event.BuyerId,
                    OrderId = @event.OrderId,
                    TotalPrice = @event.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                    OrderItems = @event.OrderItems.ToList(),
                };
                await _eventBus.SendEndPoint(stockReservedEvent);
            }
            else
            {
                //Stock operation failed...
                //Event will be thrown to warn Order.
                StockNotReservedIntegrationEvent stockNotReservedEvent = new()
                {
                    BuyerId = @event.BuyerId,
                    OrderId = @event.OrderId,
                    ErrorMessage = "Inadequate stock quantity..."
                };

                await _eventBus.Publish(stockNotReservedEvent);
            }
        }
    }
}
