using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PartyOrderLister
    {
        public List<PartyOrder> List { get; set; } = new List<PartyOrder>();

        public PartyOrder SearchCriteria { get; set; } = new PartyOrder();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
