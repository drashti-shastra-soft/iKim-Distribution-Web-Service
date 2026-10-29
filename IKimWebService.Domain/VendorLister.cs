using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class VendorLister
    {
        public List<Vendor> List { get; set; } = new List<Vendor>();

        public Vendor SearchCriteria { get; set; } = new Vendor();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
