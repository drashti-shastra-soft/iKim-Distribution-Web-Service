using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IStockUpdateService
    {
        Domain.StockUpdate Upsert(Domain.StockUpdate mStockUpdate);

        List<Domain.StockUpdate> GetBy(int itemId);
    }
}
