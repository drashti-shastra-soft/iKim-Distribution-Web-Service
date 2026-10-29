using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _repo;

        public VendorService(IVendorRepository repo)
        {
            _repo = repo;
        }

        public Domain.VendorLister GetAll(Domain.VendorLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Vendor Upsert(Domain.Vendor mVendor)
        {
            return _repo.Upsert(mVendor);
        }

        public Domain.Vendor Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }


        public List<Domain.Vendor> GetActiveVendor()
        {
            return _repo.GetActiveVendor();
        }
    }
}
