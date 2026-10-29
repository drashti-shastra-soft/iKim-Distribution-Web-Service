using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;

namespace IKimWebService.Service
{
    public class DailySalesService : IDailySalesService
    {

        private readonly IDailySalesRepository _repo;

        public DailySalesService(IDailySalesRepository repo)
        {
            _repo = repo;
        }

        public Domain.DailySalesLister GetAll(Domain.DailySalesLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.DailySales Upsert(Domain.DailySales mDailySales)
        {
            return _repo.Upsert(mDailySales);
        }

        public Domain.DailySales Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public Domain.DailySales CheckPendingSales(int storeId)
        {
            return _repo.CheckPendingSales(storeId);
        }

    }
}
