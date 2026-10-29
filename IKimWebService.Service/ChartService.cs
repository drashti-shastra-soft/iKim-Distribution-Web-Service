using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class ChartService : IChartService
    {
        private readonly IChartRepository _repo;

        public ChartService(IChartRepository repo)
        {
            _repo = repo;
        }


        public List<Domain.Data> SalesChart(int storeId)
        {
            return _repo.SalesChart(storeId);
        }


        public List<Domain.Data> PurchaseChart(int vendorId)
        {
            return _repo.PurchaseChart(vendorId);
        }

        public List<Domain.Data> IngredientConsumptionReport(int itemId)
        {
            return _repo.IngredientConsumptionReport(itemId);
        }


    }
}
