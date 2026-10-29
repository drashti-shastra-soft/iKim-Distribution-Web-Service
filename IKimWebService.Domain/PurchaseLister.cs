using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PurchaseLister
    {
        public List<Purchase> List { get; set; } = new List<Purchase>();

        public Purchase SearchCriteria { get; set; } = new Purchase();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
