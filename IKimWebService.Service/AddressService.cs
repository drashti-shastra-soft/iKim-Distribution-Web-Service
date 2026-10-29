using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;

namespace IKimWebService.Service
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly IStoreRepository _storeRepo;
        private readonly IVendorRepository _vendorRepo;

        public AddressService(IAddressRepository repo, IStoreRepository storeRepo, IVendorRepository vendorRepo)
        {
            _repo = repo;
            _storeRepo = storeRepo;
            _vendorRepo = vendorRepo;
        }

        public Domain.Address Upsert(Domain.Address mAddress)
        {
            mAddress = _repo.Upsert(mAddress);

            if (mAddress.StoreId > 0)
            {
                _storeRepo.UpdateAddress(mAddress.StoreId, mAddress.Id, mAddress.LastModifiedBy);
            }

            if (mAddress.VendorId > 0)
            {
                _vendorRepo.UpdateVendorAddress(mAddress.VendorId, mAddress.Id, mAddress.LastModifiedBy);
            }
            return mAddress;
        }

        public Domain.Address GetByStoreId(int storeId)
        {
            return _repo.GetByStoreId(storeId);
        }


        public Domain.Address GetByVendorId(int vendorId)
        {
            return _repo.GetByVendorId(vendorId);
        }
    }
}
