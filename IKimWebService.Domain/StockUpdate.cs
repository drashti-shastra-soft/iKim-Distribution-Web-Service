using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class StockUpdate : AuditTrail
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Operation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Item { get; set; }
        public string Category { get; set; }
        public string Reason { get; set; }
        public int AuditId { get; set; }
    }
}
