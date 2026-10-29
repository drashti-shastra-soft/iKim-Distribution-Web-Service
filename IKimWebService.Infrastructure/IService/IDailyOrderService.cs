using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IService
{
    public interface IDailyOrderService
    {
        List<Domain.DailyOrder> DailyOrders(Domain.DailyOrder mDailyOrder);

        string GenerateDailyOrderChallan(List<Domain.DailyOrder> mDailyOrders);

        List<Domain.DailyOrder> GetSelectedDailyOrders(List<int> mDailyOrder);
    }
}
