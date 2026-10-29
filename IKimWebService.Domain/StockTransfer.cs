using System;

namespace IKimWebService.Domain
{
    public class StockTransfer
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Item { get; set; }
        public string UnitOfMeasure { get; set; }
        public int UnitOfMeasureId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

}
