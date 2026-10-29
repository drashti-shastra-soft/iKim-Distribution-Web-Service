using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IDailySalesRepository
    {
        Domain.DailySalesLister GetAll(Domain.DailySalesLister mLister);

        Domain.DailySales Upsert(Domain.DailySales mDailySales);

        Domain.DailySales Get(int id);

        void Delete(int id, int loginUserId);

        Domain.DailySales CheckPendingSales(int storeId);
    }
}
