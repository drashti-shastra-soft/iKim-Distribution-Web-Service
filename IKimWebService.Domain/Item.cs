using System.Collections.Generic;

namespace IKimWebService.Domain
{
    public class Item : AuditTrail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UnitOfMeasureId { get; set; }
        public int CategoryId { get; set; }
        public decimal? FactoryStock { get; set; }
        public decimal? GodownStock { get; set; }
        public int HSNCode { get; set; }
        public decimal GST { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public bool IsActive { get; set; }
        public bool IsRawMaterial { get; set; }
        public int PricingTierId { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string UnitOfMeasure { get; set; }
        public int Multiply { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string ImagePath { get; set; }
        public string Base64String { get; set; }
        public bool IsRetailerCanOrder { get; set; }
        public int AuditId { get; set; }
        public string Status { get; set; }
        public List<ItemPrice> Prices { get; set; } = new List<ItemPrice>();

    }
}
