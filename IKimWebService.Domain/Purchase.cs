using System;

namespace IKimWebService.Domain
{
    public class Purchase : AuditTrail
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string BillNumber { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
        public string Status { get; set; }
    }
}
