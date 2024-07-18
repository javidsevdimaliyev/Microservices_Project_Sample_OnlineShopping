using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Shared.Events.Stock
{
    public class StockNotReservedIntegrationEvent : IntegrationEvent
    {
        public int BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
