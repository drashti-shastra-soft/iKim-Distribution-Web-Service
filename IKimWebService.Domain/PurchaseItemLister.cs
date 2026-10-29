using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PurchaseItemLister
    {
        public List<PurchaseItem> List { get; set; } = new List<PurchaseItem>();

        public PurchaseItem SearchCriteria { get; set; } = new PurchaseItem();

        public Pagination Pagination { get; set; } = new Pagination();

        public string Vendor { get; set; } = string.Empty;
    }
}
