using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IOrderLineRepository
    {
        Domain.OrderLineLister GetAll(Domain.OrderLineLister mLister);

        Domain.OrderLine Upsert(Domain.OrderLine mOrderLine);

        List<Domain.OrderLine> Get(int id);

        List<Domain.OrderLine> GetBy(int orderId);

        Domain.OrderLine GetOrderLine(int id);

        void Delete(int id, int loginUserId);
    }
}
