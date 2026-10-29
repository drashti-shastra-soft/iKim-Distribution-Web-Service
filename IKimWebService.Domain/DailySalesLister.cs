using System.Collections.Generic;

namespace IKimWebService.Domain
{
    public class DailySalesLister
    {
        public List<DailySales> List { get; set; } = new List<DailySales>();

        public DailySales SearchCriteria { get; set; } = new DailySales();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
