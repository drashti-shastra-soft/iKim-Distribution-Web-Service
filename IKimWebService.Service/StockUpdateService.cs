using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System;

namespace IKimWebService.Service
{
    public class StockUpdateService : IStockUpdateService
    {
        private readonly IStockUpdateRepository _repo;

        public StockUpdateService(IStockUpdateRepository repo)
        {
            _repo = repo;
        }

        public Domain.StockUpdate Upsert(Domain.StockUpdate mStockUpdate)
        {
            return _repo.Upsert(mStockUpdate);
        }

        public List<Domain.StockUpdate> GetBy(int itemId)
        {
            return _repo.GetBy(itemId);
        }

    }
}
