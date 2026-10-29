using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;

namespace IKimWebService.Service
{
    public class StockTransferService : IStockTransferService
    {
        private readonly IStockTransferRepository _repo;
        private readonly IItemRepository _itemRepo;

        public StockTransferService(IStockTransferRepository repo, IItemRepository itemRepo)
        {
            _repo = repo;
            _itemRepo = itemRepo;
        }

        public Domain.StockTransferLister GetAll(Domain.StockTransferLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.StockTransfer Upsert(Domain.StockTransfer mStockTransfer)
        {
            mStockTransfer = _repo.Upsert(mStockTransfer);

            if (mStockTransfer != null && mStockTransfer.Id > default(int))
            {
                _itemRepo.UpdateStock(mStockTransfer.ItemId, mStockTransfer.Quantity, mStockTransfer.CreatedBy);
            }
            return mStockTransfer;
        }

        public Domain.StockTransfer Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }
    }

}
