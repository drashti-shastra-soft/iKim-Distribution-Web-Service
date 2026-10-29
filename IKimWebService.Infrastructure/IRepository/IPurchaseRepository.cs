using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IPurchaseRepository
    {
        Domain.PurchaseLister GetAll(Domain.PurchaseLister mLister);

        Domain.Purchase Upsert(Domain.Purchase mPurchase);

        Domain.Purchase Get(int id);

        void Delete(int id, int loginUserId);
    }
}
