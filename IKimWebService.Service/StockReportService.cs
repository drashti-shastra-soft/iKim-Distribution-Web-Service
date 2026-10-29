using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class StockReportService : IStockReportService
    {
        private readonly IStockReportRepository _repo;

        public StockReportService(IStockReportRepository repo)
        {
            _repo = repo;
        }

        public Domain.StockReportLister GetAll(Domain.StockReportLister mLister)
        {
            return _repo.GetAll(mLister);
        }
    }
}
