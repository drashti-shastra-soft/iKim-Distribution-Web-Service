using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class UnitOfMeasureLister
    {
        public List<UnitOfMeasure> List { get; set; } = new List<UnitOfMeasure>();

        public UnitOfMeasure SearchCriteria { get; set; } = new UnitOfMeasure();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
