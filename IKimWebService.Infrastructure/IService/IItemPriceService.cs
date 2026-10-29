using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IItemPriceService
    {
        Domain.ItemPrice Upsert(Domain.ItemPrice mItemPrice);
        
        Domain.ItemPrice Get(int id);

        List<Domain.ItemPrice> GetItemPrices(int itemId);

        decimal GetByItemPrice(int orderId, int itemId);
    }
}
