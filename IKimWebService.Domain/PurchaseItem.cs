using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PurchaseItem : AuditTrail
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal TotalAmount { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Item { get; set; }
        public int AuditId { get; set; }
    }
}
