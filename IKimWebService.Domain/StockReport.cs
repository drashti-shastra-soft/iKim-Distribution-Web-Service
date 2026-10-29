using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class StockReport
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string FactoryStock { get; set; }
        public string GodownStock { get; set; }
    }
}
