using System;

namespace IKimWebService.Domain
{
    public class PartyOrderLine : AuditTrail
    {
        public int Id { get; set; }
        public int PartyOrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Item { get; set; }
        public string UnitOfMeasure { get; set; }
        public int Quantity { get; set; }
        public int UnitOfMeasureId { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public int AuditId { get; set; }

        public string FileName { get; set; }
        public string Base64String { get; set; }
    }
}
