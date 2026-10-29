using System;

namespace IKimWebService.Domain
{
    public class CartItem
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Item { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Base64String { get; set; }
        public int Multiply { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
