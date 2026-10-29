using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IStockUpdateRepository
    {
        Domain.StockUpdate Upsert(Domain.StockUpdate mStockUpdate);

        List<Domain.StockUpdate> GetBy(int itemId);
    }
}
