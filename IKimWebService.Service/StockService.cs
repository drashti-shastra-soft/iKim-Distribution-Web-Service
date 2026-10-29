using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _repo;

        public StockService(IStockRepository repo)
        {
            _repo = repo;
        }

        public Domain.StockLister GetAll(Domain.StockLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Stock Upsert(Domain.Stock mStock)
        {
            var mStocks = _repo.GetBy(mStock.StoreId);

            if (mStocks != null && mStocks.Count > 0)
            {
                var lastStock = mStocks.OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastStock != null)
                {
                    var daysDiff = (DateTime.Now - lastStock.EntryDate).TotalDays;
                    if (daysDiff < 7)
                        throw new APIRequestFailedException("You can add stock only after 7 days from last entry!");
                }
            }

            return _repo.Upsert(mStock);
        }

        public Domain.Stock Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Stock> GetActiveStock()
        {
            return _repo.GetActiveStock();
        }
    }
}
