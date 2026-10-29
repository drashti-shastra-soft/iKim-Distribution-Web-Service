using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IDailyOrderRepository
    {
        List<Domain.DailyOrder> DailyOrders(Domain.DailyOrder mDailyOrders);

        List<Domain.DailyOrder> GetSelectedDailyOrders(List<int> mDailyOrder);
    }
}
