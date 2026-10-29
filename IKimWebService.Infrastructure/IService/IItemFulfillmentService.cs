using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IService
{
    public interface IItemFulfillmentService
    {
        Domain.ItemFulfillment Upsert(Domain.ItemFulfillment mItemFulfillment);

        List<Domain.ItemFulfillment> GetByOrderLineId(int orderLineId);
    }
}
