using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PartyOrderLineLister
    {
        public List<PartyOrderLine> List { get; set; } = new List<PartyOrderLine>();

        public PartyOrderLine SearchCriteria { get; set; } = new PartyOrderLine();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
