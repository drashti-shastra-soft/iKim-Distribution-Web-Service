using System.Collections.Generic;
using System;

namespace IKimWebService.Domain
{
    public class Order : AuditTrail
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int StoreId { get; set; }
        public string Store { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int ItemId { get; set; }
        public int QuantityRequested { get; set; }
        public List<Domain.OrderLine> Lines { get; set; } = new List<OrderLine>();
    }
}
