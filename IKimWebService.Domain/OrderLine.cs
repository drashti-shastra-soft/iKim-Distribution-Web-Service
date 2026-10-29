using System.Collections.Generic;

namespace IKimWebService.Domain
{
    public class OrderLine : AuditTrail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public int QuantityRequested { get; set; }
        public int QuantityFulfilled { get; set; }
        public int FulfilledQuantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public string Status { get; set; }
        public int CartItemId { get; set; }
        public int AuditId { get; set; }

        public List<Domain.OrderLine> OrderLineItems { get; set; } = new List<Domain.OrderLine>();
    }
}
