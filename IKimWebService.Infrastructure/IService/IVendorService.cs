using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IVendorService
    {
        Domain.VendorLister GetAll(Domain.VendorLister mLister);

        Domain.Vendor Upsert(Domain.Vendor mVendor);

        Domain.Vendor Get(int id);

        void Delete(int id, int loginUserId);

        List<Domain.Vendor> GetActiveVendor();
    }
}
