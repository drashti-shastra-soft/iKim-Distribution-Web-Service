using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;


namespace IKimWebService.Service
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repo;

        public PurchaseService(IPurchaseRepository repo)
        {
            _repo = repo;
        }

        public Domain.PurchaseLister GetAll(Domain.PurchaseLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Purchase Upsert(Domain.Purchase mPurchase)
        {
            return _repo.Upsert(mPurchase);
        }

        public Domain.Purchase Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }
    }
}