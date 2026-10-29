using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class BannerLister
    {
        public List<Banner> List { get; set; } = new List<Banner>();

        public Banner SearchCriteria { get; set; } = new Banner();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
