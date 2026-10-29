using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IStockReportRepository
    {
        Domain.StockReportLister GetAll(Domain.StockReportLister mLister);
    }
}
