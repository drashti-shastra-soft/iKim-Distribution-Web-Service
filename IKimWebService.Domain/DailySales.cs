using System;

namespace IKimWebService.Domain
{
    public class DailySales
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DateTime Date { get; set; }
        public decimal Cash { get; set; }
        public decimal Online { get; set; }
        public string Store { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
