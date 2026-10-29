using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Linq;

namespace IKimWebService.Service
{
    public class PurchaseItemService : IPurchaseItemService
    {

        private readonly IPurchaseItemRepository _repo;
        private readonly IItemRepository _itemRepo;
        private readonly IStockUpdateRepository _stockUpdateRepo;
        private readonly IPurchaseRepository _purchaseRepo;

        public PurchaseItemService(IPurchaseItemRepository repo, IItemRepository itemRepo, IStockUpdateRepository stockUpdateRepo, IPurchaseRepository purchaseRepo)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _stockUpdateRepo = stockUpdateRepo;
            _purchaseRepo = purchaseRepo;
        }

        public Domain.PurchaseItemLister GetAll(Domain.PurchaseItemLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.PurchaseItem Upsert(Domain.PurchaseItem mPurchaseItem)
        {
            var oldQty = mPurchaseItem.Id > 0 ? _repo.GetPurchaseItem(mPurchaseItem.Id)?.Qty ?? 0 : 0;

            mPurchaseItem = _repo.Upsert(mPurchaseItem);

            var mItem = _itemRepo.Get(mPurchaseItem.ItemId);
            if (mItem == null) return mPurchaseItem;

            var qtyDiff = mPurchaseItem.Qty - oldQty;

            mItem.FactoryStock = (mItem.FactoryStock ?? 0) + qtyDiff;
            mItem.GodownStock = (mItem.GodownStock ?? 0) + qtyDiff;
            _itemRepo.Upsert(mItem);

            _stockUpdateRepo.Upsert(new Domain.StockUpdate
            {
                ItemId = mPurchaseItem.ItemId,
                Quantity = qtyDiff,
                Operation = "Plus",
                CreatedBy = mPurchaseItem.CreatedBy
            });

            var PurchaseItems = _repo.Get(mPurchaseItem.PurchaseId) ?? new List<Domain.PurchaseItem>();
            var totalAmount = PurchaseItems.Sum(x => x.TotalAmount);

            var mPurchase = _purchaseRepo.Get(mPurchaseItem.PurchaseId);
            if (mPurchase != null)
            {
                mPurchase.Amount = totalAmount;
                _purchaseRepo.Upsert(mPurchase);
            }

            return mPurchaseItem;
        }

        
        public List<Domain.PurchaseItem> Get(int id)
        {
            return _repo.Get(id);
        }

        public Domain.PurchaseItem GetPurchaseItem(int id)
        {
            return _repo.GetPurchaseItem(id);
        }

        public void Delete(int id, int loginUserId)
        {
            var mPurchaseItem = _repo.GetPurchaseItem(id);
            if (mPurchaseItem == null) return;

            // Update Item Stock if exists
            var mItem = _itemRepo.Get(mPurchaseItem.ItemId);
            if (mItem != null)
            {
                var qty = mPurchaseItem.Qty;

                mItem.FactoryStock = (mItem.FactoryStock ?? 0) - qty;
                mItem.GodownStock = (mItem.GodownStock ?? 0) - qty;
                _itemRepo.Upsert(mItem);

                _stockUpdateRepo.Upsert(new Domain.StockUpdate
                {
                    ItemId = mPurchaseItem.ItemId,
                    Quantity = qty,
                    Operation = "Minus",
                    CreatedBy = loginUserId
                });
            }

            // Delete Purchase Item
            _repo.Delete(id, loginUserId);

            // Recalculate Purchase Amount
            var mPurchaseItems = _repo.Get(mPurchaseItem.PurchaseId) ?? new List<Domain.PurchaseItem>();
            var totalAmount = mPurchaseItems.Sum(x => x.TotalAmount);

            var mPurchase = _purchaseRepo.Get(mPurchaseItem.PurchaseId);
            if (mPurchase != null)
            {
                mPurchase.Amount = totalAmount;
                _purchaseRepo.Upsert(mPurchase);
            }
        }


    }
}
