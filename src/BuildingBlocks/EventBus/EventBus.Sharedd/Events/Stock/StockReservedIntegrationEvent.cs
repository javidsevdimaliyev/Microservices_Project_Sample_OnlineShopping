using EventBus.Base.Events;
using EventBus.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Shared.Events.Stock
{
    public class StockReservedIntegrationEvent : IntegrationEvent
    {
        public int BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
