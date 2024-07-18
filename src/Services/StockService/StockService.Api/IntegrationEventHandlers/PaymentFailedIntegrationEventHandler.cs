using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Payment;
using MassTransit;
using MongoDB.Driver;
using StockService.Api.Infrastructure;

namespace StockService.Api.IntegrationEventHandlers
{
    public class PaymentFailedIntegrationEventHandler(MongoDBRepository mongoDBService) : IIntegrationEventHandler<PaymentFailedIntegrationEvent>, IConsumer<PaymentFailedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            await Handle(context.Message);
        }
        public async Task Handle(PaymentFailedIntegrationEvent @event)
        {
            var stocks = mongoDBService.GetCollection<Infrastructure.Models.Stock>();
            foreach (var orderItem in @event.OrderItems)
            {
                var stock = await (await stocks.FindAsync(s => s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                if (stock != null)
                {
                    stock.Count += orderItem.Quantity;
                    await stocks.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId.ToString(), stock);
                }
            }
        }
    }
}
