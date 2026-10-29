using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class StockReportLister
    {
        public List<StockReport> List { get; set; } = new List<StockReport>();
        public StockReport SearchCriteria { get; set; } = new StockReport();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}
