using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IPurchaseItemService
    {
        Domain.PurchaseItemLister GetAll(Domain.PurchaseItemLister mLister);

        Domain.PurchaseItem Upsert(Domain.PurchaseItem mPurchaseItem);

        List<Domain.PurchaseItem> Get(int id);

        Domain.PurchaseItem GetPurchaseItem(int id);

        void Delete(int id, int loginUserId);
    }
}
