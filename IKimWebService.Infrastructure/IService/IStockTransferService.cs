using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IStockTransferService
    {
        Domain.StockTransferLister GetAll(Domain.StockTransferLister mLister);

        Domain.StockTransfer Upsert(Domain.StockTransfer mStockTransfer);

        Domain.StockTransfer Get(int id);

        void Delete(int id, int loginUserId);
    }
}
